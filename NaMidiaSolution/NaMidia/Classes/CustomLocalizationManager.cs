using Telerik.Windows.Controls;

namespace NaMidia
{
    public class CustomLocalizationManager : LocalizationManager
    {
        public override string GetStringOverride(string key)
        {
            switch (key)
            {
                // RadGridView
                case "GridViewAlwaysVisibleNewRow":
                    return "Clique aqui para adicionar um novo item.";

                case "GridViewGroupPanelText":
                    return "Para agrupar, arraste a coluna até aqui.";

                case "GridViewClearFilter":
                    return "Limpar Filtros";

                case "GridViewFilterShowRowsWithValueThat":
                    return "Mostrar registro(s) que:";

                case "GridViewFilterSelectAll":
                    return "Selecionar todos";

                case "GridViewFilterContains":
                    return "Contém";

                case "GridViewFilterDoesNotContain":
                    return "Não contém";

                case "GridViewFilterStartsWith":
                    return "Começa com";

                case "GridViewFilterEndsWith":
                    return "Termina com";

                case "GridViewFilterIsContainedIn":
                    return "Inclui ";

                case "GridViewFilterIsNotContainedIn":
                    return "Não inclui ";

                case "GridViewFilterIsEqualTo":
                    return "Igual a";

                case "GridViewFilterIsGreaterThan":
                    return "Maior que";

                case "GridViewFilterIsGreaterThanOrEqualTo":
                    return "Maior ou igual  a";

                case "GridViewFilterIsLessThan":
                    return "Menor que";

                case "GridViewFilterIsLessThanOrEqualTo":
                    return "Menor ou igual a";

                case "GridViewFilterIsNotEqualTo":
                    return "Diferente de";

                case "GridViewFilterIsNull":
                    return "É nulo";

                case "GridViewFilterIsNotNull":
                    return "Não é nulo";

                case "GridViewFilterIsEmpty":
                    return "É vazio";

                case "GridViewFilterIsNotEmpty":
                    return "Não é vazio";

                case "GridViewFilterAnd":
                    return "e";

                case "GridViewFilterOr":
                    return "ou";

                case "GridViewFilterDistinctValueNull":
                    return "Nulo";

                case "GridViewFilterDistinctValueStringEmpty":
                    return "Vazio";

                case "GridViewFilterMatchCase":
                    return "Diferenciar maiúsculo de minúsculo";

                case "GridViewGroupPanelTopText":
                    return "Cabeçalho do grupo";

                case "GridViewGroupPanelTopTextGrouped":
                    return "Agrupados por:";

                case "GridViewFilter":
                    return "Filtrar";

                // RadDataPager
                case "RadDataPagerPage":
                    return "Página";

                case "RadDataPagerOf":
                    return "de";

                case "EnterDate":
                    return "Entre com a data";
            }

            return base.GetStringOverride(key);
        }
    }
}