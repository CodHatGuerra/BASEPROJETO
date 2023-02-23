using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Telerik.Windows.Controls;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace NaMidia.Classes
{
    #region [ Classe ExtensaoDeMetodos ]

    public static class ExtensaoDeMetodos
    {
        /*********************************************
        -- Classe Responsável por armazenar os
        -- Extention Methods que fazem a validação do
        -- Elemento passado por parâmetro.    
        *********************************************/

        public static void SetaValidacao(this FrameworkElement frameworkElement, string mensagem)
        {
            ValidacaoGenerica validacaoGenerica = new ValidacaoGenerica(mensagem);

            Binding binding = new Binding("ErroValidacao")
            {
                Mode = System.Windows.Data.BindingMode.TwoWay,
                NotifyOnValidationError = true,
                ValidatesOnExceptions = true,
                Source = validacaoGenerica
            };
            frameworkElement.SetBinding(Control.TagProperty, binding);
        }

        public static void RaiseErroValidacao(this FrameworkElement frameworkElement)
        {
            BindingExpression b = frameworkElement.GetBindingExpression(Control.TagProperty);

            if (b != null)
            {
                ((ValidacaoGenerica)b.DataItem).ExibeMsgErro = true;
                b.UpdateSource();
            }
        }

        public static void LimpaErrosValidacao(this FrameworkElement frameworkElement)
        {
            BindingExpression b = frameworkElement.GetBindingExpression(Control.TagProperty);

            if (b != null)
            {
                ((ValidacaoGenerica)b.DataItem).ExibeMsgErro = false;
                b.UpdateSource();
            }
        }

        /// <summary>
        /// Retorna true caso tenha erro de validacao
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <returns></returns>
        public static bool VerificarErrosValidacao(this FrameworkElement frameworkElement)
        {
            BindingExpression b = frameworkElement.GetBindingExpression(Control.TagProperty);

            if (b != null)
                return ((ValidacaoGenerica)b.DataItem).ExibeMsgErro;
            else
                return false;
        }

        public static bool IsTextValid(this string inputText)
        {
            bool isTextValid = true;

            foreach (char character in inputText)
            {
                if (char.IsWhiteSpace(character) == false)
                {
                    if (char.IsLetterOrDigit(character) == false)
                    {
                        if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                        {
                            isTextValid = false;
                            break;
                        }
                    }
                }
            }
            return isTextValid;
        }

        public static bool IsNumberValid(this string inputNumber)
        {
            bool isNumberValid = true;
            int number = -1;
            if (!Int32.TryParse(inputNumber, out number))
            {
                isNumberValid = false;
            }
            return isNumberValid;
        }

        public static bool IsEmailValid(this string inputEmail)
        {
            bool isEmailValid = true;
            string emailExpression = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
            Regex re = new Regex(emailExpression);
            if (!re.IsMatch(inputEmail))
            {
                isEmailValid = false;
            }
            return isEmailValid;
        }

        public static bool ValidaCPF(this string cpf)
        {
            bool valida = true;
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            string verifica;
            int soma;
            int resto;

            if (cpf.Length == 11)
            {
                verifica = cpf.Substring(9);
                tempCpf = cpf.Substring(0, 9);

                soma = 0;

                for (int i = 0; i < 9; i++)
                {
                    soma = soma + int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
                }

                resto = soma % 11;

                resto = resto < 2 ? resto = 0 : resto = 11 - resto; ;

                digito = resto.ToString();

                tempCpf = tempCpf + digito;

                soma = 0;
                for (int i = 0; i < 10; i++)
                {
                    soma = soma + int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
                }

                resto = soma % 11;

                resto = resto < 2 ? resto = 0 : resto = 11 - resto; ;

                digito = digito + resto.ToString();

                if (digito != verifica)
                {
                    valida = false;
                }
                switch (cpf)
                {
                    case "00000000000":
                        {
                            valida = false;
                            break;
                        }
                    case "11111111111":
                        {
                            valida = false;
                            break;
                        }
                    case "22222222222":
                        {
                            valida = false;
                            break;
                        }
                    case "33333333333":
                        {
                            valida = false;
                            break;
                        }
                    case "44444444444":
                        {
                            valida = false;
                            break;
                        }
                    case "55555555555":
                        {
                            valida = false;
                            break;
                        }
                    case "66666666666":
                        {
                            valida = false;
                            break;
                        }
                    case "77777777777":
                        {
                            valida = false;
                            break;
                        }
                    case "88888888888":
                        {
                            valida = false;
                            break;
                        }
                    case "99999999999":
                        {
                            valida = false;
                            break;
                        }
                }
            }
            else
            {
                valida = false;
            }
            return valida;
        }

        public static bool ValidaCnpj(this string cnpj)
        {
            bool valida = true;
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCnpj;
            string digito;
            string verifica;
            int soma;
            int resto;

            // System.Text.RegularExpressions.Regex.IsMatch(cnpj, (@"\d{2}.?\d{3}.?\d{3}/?\d{4}-?\d{2}"));

            if (cnpj.Length == 14)
            {
                verifica = cnpj.Substring(12);
                tempCnpj = cnpj.Substring(0, 12);
                soma = 0;

                for (int i = 0; i < 12; i++)
                {
                    soma = soma + int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
                }

                resto = soma % 11;
                resto = resto < 2 ? resto = 0 : resto = 11 - resto; ;
                digito = resto.ToString();

                tempCnpj = tempCnpj + digito;

                soma = 0;
                for (int i = 0; i < 13; i++)
                {
                    soma = soma + int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
                }

                resto = soma % 11;
                resto = resto < 2 ? resto = 0 : resto = 11 - resto; ;
                digito = digito + resto.ToString();

                if (digito != verifica)
                {
                    valida = false;
                }

                switch (cnpj)
                {
                    case "00000000000000":
                        {
                            valida = false;
                            break;
                        }

                    case "11111111111111":
                        {
                            valida = false;
                            break;
                        }
                    case "22222222222222":
                        {
                            valida = false;
                            break;
                        }
                    case "33333333333333":
                        {
                            valida = false;
                            break;
                        }
                    case "44444444444444":
                        {
                            valida = false;
                            break;
                        }
                    case "55555555555555":
                        {
                            valida = false;
                            break;
                        }
                    case "66666666666666":
                        {
                            valida = false;
                            break;
                        }
                    case "77777777777777":
                        {
                            valida = false;
                            break;
                        }
                    case "88888888888888":
                        {
                            valida = false;
                            break;
                        }
                    case "99999999999999":
                        {
                            valida = false;
                            break;
                        }
                }
            }
            else
            {

                valida = false;
            }
            return valida;
        }

        public static bool ValidaCEP(this string CEP)
        {
            //var expCep = @"/\d{5}-\d{3}";
            Regex re1 = new Regex(@"\d{5}\d{3}");
            Regex re2 = new Regex(@"\d{5}-\d{3}");


            if (!re1.IsMatch(CEP) && !re2.IsMatch(CEP))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string FormatCPF(this string cpf)
        {
            if (cpf.Length == 11)
            {  //000.000.000-00
                cpf = cpf.Substring(0, 3) + "." + cpf.Substring(3, 3) + "." + cpf.Substring(6, 3) + "-" + cpf.Substring(9, 2);
            }
            else
            {

            }
            return cpf;
        }

        public static string FormatCNPJ(this string cnpj)
        {

            if (cnpj.Length == 14)
            {  //23.756.116/0001-50
                cnpj = cnpj.Substring(0, 2) + "." + cnpj.Substring(2, 3) + "." + cnpj.Substring(5, 3) + "/" + cnpj.Substring(8, 4) + "-" + cnpj.Substring(12, 2);
            }
            return cnpj;
        }

        public static string RemoveFormatNumero(this string numero)
        {
            if (numero.Length > 0 || numero != "")
            {
                numero = numero.Replace(".", "");
                numero = numero.Replace("-", "");
                numero = numero.Replace("/", "");
            }
            return numero;
        }

        public static string InverterString(this string str)
        {
            char[] arrChar = str.ToCharArray();
            Array.Reverse(arrChar);
            string invertida = new String(arrChar);

            return invertida;
        }

        #region [ Recuperar Controles do DataTemplate ]

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object RecuperarControleDoDataTemplate(this ContentControl contentControl, string nomeControle)
        {
            ContentPresenter apresentador =
                VisualTreeHelper.GetChild(contentControl, 0) as ContentPresenter;

            if (apresentador == null)
            {
                FrameworkElement element = VisualTreeHelper.GetChild(contentControl, 0) as FrameworkElement;
                apresentador = element.ChildrenOfType<ContentPresenter>().FirstOrDefault();
            }

            return contentControl.ContentTemplate.FindName(nomeControle, apresentador);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T RecuperarControleDoDataTemplate<T>(this ContentControl contentControl, string nomeControle) where T : DependencyObject
        {
            ContentPresenter apresentador =
                VisualTreeHelper.GetChild(contentControl, 0) as ContentPresenter;

            if (apresentador == null)
            {
                FrameworkElement element = VisualTreeHelper.GetChild(contentControl, 0) as FrameworkElement;
                apresentador = element.ChildrenOfType<ContentPresenter>().FirstOrDefault();
            }

            return (T)contentControl.ContentTemplate.FindName(nomeControle, apresentador);
        }

        #endregion
    }

    #endregion

    #region [ Classe ValidacaoGenerica ]
    /*********************************************
     -- Classe Responsável por lançar 
     -- excessoes de validação    
    *********************************************/
    public class ValidacaoGenerica
    {
        private string mensagem;

        #region Propriedades
        public bool ExibeMsgErro { get; set; }

        public object ErroValidacao
        {
            get
            {
                return null;
            }
            [DebuggerHidden]
            set
            {
                if (ExibeMsgErro)
                {
                    //É necessário Configurar o Visual Studio para Ignorar Erros da Classe ValidationException
                    //Siga os passos em:http://tinyurl.com/439y849
                    throw new ValidationException(mensagem);
                    //throw new Exception(mensagem);
                }
            }
        }
        #endregion

        public ValidacaoGenerica(string msg)
        {
            this.mensagem = msg;
        }
    }
    #endregion
}
