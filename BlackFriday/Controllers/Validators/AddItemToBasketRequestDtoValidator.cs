using BlackFriday.Application.Persistence.Abstraction;
using BlackFriday.Infrastructure.Controllers.Dtos;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Infrastructure.Controllers.Validators;

public class AddItemToBasketRequestDtoValidator : AbstractValidator<AddItemToBasketRequestDto>
{
	public const string BadRequestErrorCode = "badRequest";
	public const string NotFoundErrorCode = "notFound";
	public const string PreConditionErrorCode = "preConditionNotMet";

	public AddItemToBasketRequestDtoValidator(IBlackFridayDbContextFactory dbContextFactory)
	{
		RuleFor(x => x).MustAsync(async (x, cancellationToken) =>
			{
				using var dbContext = dbContextFactory.MakeDbContext();
				return await dbContext.Products.AnyAsync(product => product.Asin == x.ProductId, cancellationToken);
			})
			.WithErrorCode(NotFoundErrorCode)
			.WithMessage("item does not exists")
			.WithName("product")
			.DependentRules(() =>
			{
				RuleFor(x => x).MustAsync(async (x, cancellationToken) =>
					{
						using var dbContext = dbContextFactory.MakeDbContext();
						return !await dbContext.Baskets
							.AnyAsync(basket => basket.ProductId == x.ProductId
									&& basket.BasketId == x.BasketId
									&& basket.UserId == x.UserId,
								cancellationToken: cancellationToken);
					})
					.WithErrorCode(BadRequestErrorCode)
					.WithMessage("item is duplicate")
					.WithName("basket")
					.DependentRules(() =>
					{
						RuleFor(x => x).MustAsync(async (request, cancellationToken) =>
							{
								using var dbContext = dbContextFactory.MakeDbContext();
								var productCount = await dbContext.ProductCounts.FirstAsync(x => x.Asin == request.ProductId, cancellationToken: cancellationToken);
								return productCount.Count > 0;
							})
							.WithErrorCode(PreConditionErrorCode)
							.WithMessage("not enough items")
							.WithName("inventory");
					});
			});
	}
}
