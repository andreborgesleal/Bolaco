using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;

namespace DWM.Models.Repositories
{
    public class ClienteViewModel : Repository
    {
        [DisplayName("ID")]
        public int clienteId { get; set; }

        [DisplayName("Nome")]
        [Required(ErrorMessage = "O campo Nome do cliente dever ser informado")]
        [StringLength(60, ErrorMessage = "O nome do cliente deve ter no mínimo 10 e no máximo 60 caracteres", MinimumLength = 10)]
        public string nome { get; set; }

        [DisplayName("CPF")]
        [Required(ErrorMessage = "O CPF do cliente deve ser informado")]
        public string cpf { get; set; }

        [DisplayName("E-mail")]
        [Required(ErrorMessage = "E-mail do cliente deve ser informado")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Informe um e-mail válido")]
        [EmailAddress(ErrorMessage = "Informe o E-mail com um formato válido")]
        public string email { get; set; }

        [DisplayName("Telefone")]
        [Required(ErrorMessage = "O telefone de contato deve ser informado")]
        public string telefone { get; set; }

        [DisplayName("Logradouro")]
        [StringLength(60, ErrorMessage = "O Endereço deve ter no máximo 60 caracteres")]
        public string endereco { get; set; }

        [DisplayName("Complemento")]
        [StringLength(25, ErrorMessage = "O Complemento do endereço deve ter no máximo 25 caracteres")]
        public string complemento { get; set; }

        [DisplayName("CEP")]
        public string cep { get; set; }

        [DisplayName("Cidade")]
        [StringLength(25, ErrorMessage = "A Cidade deve ter no máximo 25 caracteres")]
        public string cidade { get; set; }

        [DisplayName("UF")]
        [StringLength(2, ErrorMessage = "A UF deve possuir 2 caracteres", MinimumLength = 2)]
        public string uf { get; set; }

        [DisplayName("Usuário")]
        public Nullable<int> usuarioId { get; set; }

        public string nome_usuario { get; set; }
    }
}