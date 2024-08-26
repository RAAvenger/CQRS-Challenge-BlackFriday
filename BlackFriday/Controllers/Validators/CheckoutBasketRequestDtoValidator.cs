using BlackFriday.Application.Persistence.Abstraction;
using BlackFriday.Infrastructure.Controllers.Dtos;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Infrastructure.Controllers.Validators;

public class CheckoutBasketRequestDtoValidator : AbstractValidator<CheckoutBasketRequestDto>
{
	public const string NotFoundErrorCode = "notFound";
	public const string PreConditionErrorCode = "preConditionNotMet";

	public CheckoutBasketRequestDtoValidator(IBlackFridayDbContextFactory dbContextFactory)
	{
		RuleFor(x => x).MustAsync(async (x, cancellationToken) =>
		{
			using var dbContext = dbContextFactory.MakeDbContext();
			var basketItems = await dbContext.Baskets
				.Where(x => x.BasketId == x.BasketId && x.UserId == x.UserId && !x.IsCheckedOut)
				.CountAsync(cancellationToken);
			return basketItems > 0;
		})
			.WithErrorCode(NotFoundErrorCode)
			.WithMessage("basket not found")
		.WithName("basket")
			.DependentRules(() =>
			{
				RuleFor(x => x).MustAsync(async (x, cancellationToken) =>
				{
					using var dbContext = dbContextFactory.MakeDbContext();
					return await dbContext.Baskets
						.Where(basket => basket.BasketId == x.BasketId && basket.UserId == x.UserId && !basket.IsCheckedOut)
						.Join(dbContext.ProductCounts,
							basket => basket.ProductId,
							productCount => productCount.Asin,
							(basket, productCount) => productCount.Count)
						.AllAsync(x => x > 0, cancellationToken);
				})
					.WithErrorCode(PreConditionErrorCode)
					.WithMessage("not enough items")
					.WithName("inventory");
			});
	}
}
