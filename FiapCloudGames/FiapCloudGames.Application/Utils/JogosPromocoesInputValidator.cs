using FluentValidation;
using FiapCloudGames.Application.DTOs;

namespace FiapCloudGames.Application.Utils
{
    public class JogosPromocoesInputValidator : AbstractValidator<JogosPromocoesDTO>
    {
        public JogosPromocoesInputValidator()
        {
            RuleFor(x => x.JogoId)
                .GreaterThan(0)
                .WithMessage("Id do jogo tem que ser > 0.");

            RuleFor(x => x.PromocaoId)
                .GreaterThan(0)
                .WithMessage("Id da promoção tem que ser > 0.");

            RuleFor(x => x.Desconto)
                .GreaterThan(0)
                .WithMessage("Desconto tem que ser > 0.")
                .LessThanOrEqualTo(100)
                .WithMessage("Desconto tem que ser <= 100.");
        }
    }
}