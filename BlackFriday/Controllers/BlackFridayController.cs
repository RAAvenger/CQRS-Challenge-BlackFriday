using System.Net;
using BlackFriday.Application.Persistence.Abstraction;
using BlackFriday.Application.UseCases;
using BlackFriday.Infrastructure.Controllers.Dtos;
using BlackFriday.Infrastructure.Controllers.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace BlackFriday.Infrastructure.Controllers;

[ApiController]
public class BlackFridayController : ControllerBase
{
	private readonly IValidator<AddItemToBasketRequestDto> _addItemToBasketRequestValidator;
	private readonly IValidator<CheckoutBasketRequestDto> _checkoutBasketRequestValidator;
	private readonly IMediator _mediator;

	public BlackFridayController(IMediator mediator,
		IValidator<AddItemToBasketRequestDto> addItemToBasketRequestValidator,
		IValidator<CheckoutBasketRequestDto> checkoutBasketRequestValidator)
	{
		_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		_addItemToBasketRequestValidator = addItemToBasketRequestValidator ?? throw new ArgumentNullException(nameof(addItemToBasketRequestValidator));
		_checkoutBasketRequestValidator = checkoutBasketRequestValidator ?? throw new ArgumentNullException(nameof(checkoutBasketRequestValidator));
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
		var validationResult = await _checkoutBasketRequestValidator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			if (validationResult.Errors.Any(x => x.ErrorCode == CheckoutBasketRequestDtoValidator.NotFoundErrorCode))
			{
				validationResult.AddToModelState(ModelState);
				return ValidationProblem(statusCode: (int)HttpStatusCode.NotFound, modelStateDictionary: ModelState);
			}
			if (validationResult.Errors.Any(x => x.ErrorCode == CheckoutBasketRequestDtoValidator.PreConditionErrorCode))
			{
				validationResult.AddToModelState(ModelState);
				return ValidationProblem(statusCode: (int)HttpStatusCode.PreconditionFailed, modelStateDictionary: ModelState);
			}
		}
		await _mediator.Send(new CheckoutBasketCommand
		{
			BasketId = request.BasketId,
			UserId = request.UserId
		}, cancellationToken);
		return Ok();
	}

	[HttpGet("categories")]
	public async Task<ActionResult<IReadOnlyCollection<string>>> GetAllCategories(CancellationToken cancellationToken)
	{
		var categories = await _mediator.Send(new GetAllCategoryNamesQuery(), cancellationToken);
		return Ok(categories);
	}

	[HttpGet("items/{id}")]
	public async Task<ActionResult<Product>> GetItemById([FromRoute] string id, CancellationToken cancellationToken)
	{
		var item = await _mediator.Send(new GetProductByIdQuery { Id = id }, cancellationToken);
		if (item is null)
		{
			return NotFound();
		}
		return Ok(item);
	}

	[HttpGet("categories/{category}")]
	public async Task<ActionResult<IReadOnlyCollection<Product>>> GetItemsForGivenCategory([FromRoute] string category, CancellationToken cancellationToken)
	{
		var categoryItems = await _mediator.Send(new GetProductsOfCategoryQuery { Id = category }, cancellationToken);
		if (categoryItems.Count == 0)
		{
			return NotFound();
		}
		return Ok(categoryItems);
	}
}
