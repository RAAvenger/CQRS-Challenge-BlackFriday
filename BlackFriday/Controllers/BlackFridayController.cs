using System.Net;
using System.Text.Json;
using BlackFriday.Application.Persistence.Abstraction;
using BlackFriday.Application.UseCases;
using BlackFriday.Infrastructure.Controllers.Dtos;
using BlackFriday.Infrastructure.Controllers.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Infrastructure.Controllers;

[ApiController]
public class BlackFridayController : ControllerBase
{
	private readonly IValidator<AddItemToBasketRequestDto> _addItemToBasketRequestValidator;
	private readonly IBlackFridayDbContext _dbContext;
	private readonly IMediator _mediator;

	public BlackFridayController(IBlackFridayDbContextFactory dbContextFactory,
		IMediator mediator,
		IValidator<AddItemToBasketRequestDto> addItemToBasketRequestValidator)
	{
		_dbContext = dbContextFactory?.MakeDbContext() ?? throw new ArgumentNullException(nameof(dbContextFactory));
		_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		_addItemToBasketRequestValidator = addItemToBasketRequestValidator ?? throw new ArgumentNullException(nameof(addItemToBasketRequestValidator));
	}

	[HttpPost("add-item-to-basket")]
	public async Task<ActionResult> AddItemToBasket([FromBody] AddItemToBasketRequestDto request,
		CancellationToken cancellationToken)
	{
		var validationResult = await _addItemToBasketRequestValidator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			if (validationResult.Errors.Any(x => x.ErrorCode == AddItemToBasketRequestDtoValidator.NotFoundErrorCode))
			{
				validationResult.AddToModelState(ModelState);
				return ValidationProblem(statusCode: (int)HttpStatusCode.NotFound, modelStateDictionary: ModelState);
			}
			if (validationResult.Errors.Any(x => x.ErrorCode == AddItemToBasketRequestDtoValidator.BadRequestErrorCode))
			{
				validationResult.AddToModelState(ModelState);
				return ValidationProblem(statusCode: (int)HttpStatusCode.BadRequest, modelStateDictionary: ModelState);
			}

			if (validationResult.Errors.Any(x => x.ErrorCode == AddItemToBasketRequestDtoValidator.PreConditionErrorCode))
			{
				validationResult.AddToModelState(ModelState);
				return ValidationProblem(statusCode: (int)HttpStatusCode.PreconditionFailed, modelStateDictionary: ModelState);
			}
		}
		await _mediator.Send(new AddItemToBasketCommand
		{
			BasketId = request.BasketId,
			ProductId = request.ProductId,
			UserId = request.UserId
		}, cancellationToken);

		return Ok();
	}

	[HttpPost("checkout-basket")]
	public async Task<ActionResult> CheckoutBasket([FromBody] CheckoutBasketRequestDto request,
		CancellationToken cancellationToken)
	{
		var basketItems = await _dbContext.Baskets
			.Where(x => x.BasketId == request.BasketId && x.UserId == request.UserId && !x.IsCheckedOut)
			.ToArrayAsync(cancellationToken);
		if (basketItems.Length == 0)
		{
			return NotFound();
		}

		var itemCounts = new Dictionary<string, ProductCount>();
		foreach (var item in basketItems)
		{
			var count = await _dbContext.ProductCounts.FirstAsync(x => x.Asin == item.ProductId, cancellationToken: cancellationToken);
			if (count.Count == 0)
			{
				return StatusCode((int)HttpStatusCode.PreconditionFailed, "not enough items");
			}
			itemCounts.Add(count.Asin, count);
		}

		foreach (var item in basketItems)
		{
			item.IsCheckedOut = true;
		}
		var itemsJson = JsonSerializer.Serialize(basketItems.Select(x => x.ProductId).ToArray());
		_dbContext.Invoices.Add(new Invoice
		{
			BasketId = request.BasketId,
			UserId = request.UserId,
			Items = itemsJson
		});

		await _dbContext.SaveChangesAsync(cancellationToken);
		foreach (var item in basketItems)
		{
			await _dbContext.ProductCounts
				.Where(x => x.Asin == item.ProductId)
				.ExecuteUpdateAsync(x => x.SetProperty(productCount => productCount.Count,
						productCount => productCount.Count + 1),
					cancellationToken: cancellationToken);
		}
		return Ok();
	}

	[HttpGet("categories")]
	public async Task<ActionResult<IReadOnlyCollection<string>>> GetAllCategories(CancellationToken cancellationToken)
	{
		var categories = await _dbContext.Products
			.GroupBy(x => x.CategoryName)
			.Select(x => x.Key)
			.ToArrayAsync(cancellationToken);

		return Ok(categories);
	}

	[HttpGet("items/{id}")]
	public async Task<ActionResult<Product>> GetItemById([FromRoute] string id, CancellationToken cancellationToken)
	{
		var item = await _dbContext.Products
			.FirstOrDefaultAsync(x => x.Asin == id, cancellationToken);
		if (item is null)
		{
			return NotFound();
		}
		return Ok(item);
	}

	[HttpGet("categories/{category}")]
	public async Task<ActionResult<IReadOnlyCollection<Product>>> GetItemsForGivenCategory([FromRoute] string category, CancellationToken cancellationToken)
	{
		var categoryItems = await _dbContext.Products
			.Where(x => x.CategoryName == category)
			.ToArrayAsync(cancellationToken);
		if (categoryItems.Length == 0)
		{
			return NotFound();
		}
		return Ok(categoryItems);
	}
}
