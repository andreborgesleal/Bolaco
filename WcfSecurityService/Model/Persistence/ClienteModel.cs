using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Models;
using App_Dominio.Security;
using App_Dominio.Repositories;

namespace DWM.Models.Persistence
{
    public class ClienteModel : CrudContext<Cliente, ClienteViewModel, ApplicationContext>
    {

        public  ClienteModel() : base()
        {

        }

        public ClienteModel(ApplicationContext _db)
        {
            this.db = _db;
        }

        #region Métodos da classe CrudContext
        public override Cliente MapToEntity(ClienteViewModel value)
        {
            Cliente entity = new Cliente()
            {
                clienteId = value.clienteId,
                nome = value.nome.ToUpper(),
                cpf = value.cpf.Replace(".","").Replace("-",""),
                email = value.email,
                telefone = value.telefone.Replace("-","").Replace(" ",""),
                usuarioId = value.usuarioId.Value
            };

            if (value.endereco != null && value.endereco != "")
            {
                entity.endereco = value.endereco != null ? value.endereco.ToUpper() : value.endereco ;
                entity.complemento = value.complemento != null ? value.complemento.ToUpper() : value.complemento;
                entity.cidade = value.cidade != null ? value.cidade.ToUpper() : value.cidade;
                entity.cep = value.cep != null ? value.cep.Replace("-","") : value.cep;
                entity.uf = value.uf != null ? value.uf.ToUpper() : "";
            }

            return entity;
        }

        public override ClienteViewModel MapToRepository(Cliente entity)
        {
            return new ClienteViewModel()
            {
                clienteId = entity.clienteId,
                nome = entity.nome,
                cpf = entity.cpf,
                email = entity.email,
                telefone = entity.telefone,
                endereco = entity.endereco,
                complemento = entity.complemento,
                cidade = entity.cidade,
                cep = entity.cep,
                uf = entity.uf,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Cliente Find(ClienteViewModel key)
        {
            return db.Clientes.Find(key.clienteId);
        }

        public override Validate Validate(ClienteViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            if (value.nome.Trim().Length == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Nome da Cliente").ToString();
                value.mensagem.MessageBase = "Campo Nome deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            #region verifica se o nome do cliente está abreviado
            if (value.nome.Contains(".") || value.nome.Contains("/"))
            {
                value.mensagem.Code = 4;
                value.mensagem.Message = MensagemPadrao.Message(4, "Nome", "Informar o nome completo sem abreviações").ToString();
                value.mensagem.MessageBase = "Nome do cliente inválido.";
                return value.mensagem;
            }
            #endregion

            #region valida cpf
            if (!Funcoes.ValidaCpf(value.cpf.Replace(".", "").Replace("-", "")))
            {
                value.mensagem.Code = 29;
                value.mensagem.Message = MensagemPadrao.Message(29).ToString();
                value.mensagem.MessageBase = "Número de CPF incorreto.";
                return value.mensagem;
            }

            if (operation == Crud.ALTERAR)
            {
                if (db.Clientes.Where(info => info.cpf == value.cpf.Replace(".", "").Replace("-", "") && info.clienteId != value.clienteId).Count() > 0)
                {
                    value.mensagem.Code = 31;
                    value.mensagem.Message = MensagemPadrao.Message(31).ToString();
                    value.mensagem.MessageBase = "CPF informado já se encontra cadastrado.";
                    return value.mensagem;
                }
            }
            else
            {
                if (db.Clientes.Where(info => info.cpf == value.cpf.Replace(".", "").Replace("-", "")).Count() > 0)
                {
                    value.mensagem.Code = 31;
                    value.mensagem.Message = MensagemPadrao.Message(31).ToString();
                    value.mensagem.MessageBase = "CPF informado já se encontra cadastrado.";
                    return value.mensagem;
                }
            }
            #endregion

            #region Verifica se o e-mail do cliente já foi atribuído para outro cliente
            if (operation == Crud.ALTERAR)
            {
                if (db.Clientes.Where(info => info.email == value.email && info.clienteId != value.clienteId).Count() > 0)
                {
                    value.mensagem.Code = 41;
                    value.mensagem.Message = MensagemPadrao.Message(41, "E-mail").ToString();
                    value.mensagem.MessageBase = "E-mail informado já se encontra cadastrado.";
                    return value.mensagem;
                }
            }
            else if (operation == Crud.INCLUIR)
            {
                if (db.Clientes.Where(info => info.email == value.email).Count() > 0)
                {
                    value.mensagem.Code = 41;
                    value.mensagem.Message = MensagemPadrao.Message(41, "E-mail").ToString();
                    value.mensagem.MessageBase = "E-mail informado já se encontra cadastrado.";
                    return value.mensagem;
                }
            }
            #endregion

            return value.mensagem;
        }

        #endregion


        #region Métodos customizados
        public ClienteViewModel getClienteByUsuario(int usuarioId)
        {
            using (db = getContextInstance())
            {
                Cliente entity = db.Clientes.Where(info => info.usuarioId == usuarioId).FirstOrDefault();
                if (entity != null)
                    return MapToRepository(entity);
                else
                    return new ClienteViewModel();
            }
        }

        public int? getClienteByLogin(string login, EmpresaSecurity<App_DominioContext> security)
        {
            int? clienteId = null;

            #region retorna o usuário para verificar se o mesmo é um condômino
            using (ApplicationContext db = new ApplicationContext())
            {
                int empresaId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EMPRESA).valor);
                UsuarioRepository usuarioRepository = security.getUsuarioByLogin(login, empresaId);

                if (usuarioRepository != null)
                    if (db.Clientes.Where(info => info.usuarioId == usuarioRepository.usuarioId).Count() > 0)
                    {
                        Cliente a = db.Clientes.Where(info => info.usuarioId == usuarioRepository.usuarioId).FirstOrDefault();
                        clienteId = a.clienteId;
                    }
            }
            #endregion

            return clienteId;
        }

        #endregion
    }
}