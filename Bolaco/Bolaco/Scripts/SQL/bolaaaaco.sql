use seguranca
go

select * from seguranca..Empresa
select * from seguranca..Sistema
select * from seguranca..EmpresaSistema
select * from seguranca..Grupo
select * from seguranca..Usuario where empresaId = 4

select * from Transacao where sistemaId = 5 and url like 'Account%' or transacaoId = 249

begin tran

declare @usuarioId int 
declare @grupoAdmId int, @grupoClienteId int

insert into Sistema values(6, 'Bolaaaaço da Norte Refrigeração', 'Bolaaaaço da Norte Refrigeração')
insert into Empresa values(4, 'Norte Refrigeração', 'bolaaaaco@norterefrigeracao.com.br')
insert into EmpresaSistema values(4, 6, 'A')
insert into EmpresaSistema values(4, 1, 'A')

insert into Grupo values(6, 4, 'Administração', 'A')
select @grupoAdmId = @@IDENTITY

insert into Grupo values(6, 4, 'Cliente', 'A')
select @grupoClienteId = @@IDENTITY

insert into Usuario values(4, 'zecadbleal@gmail.com', 'Antônio José D B Leal', GETDATE(), 'A', 'S', '9932CD7B406BDFC3F36E399A59428183DADFCFFF', null, null)
select @usuarioId = @@IDENTITY
insert into UsuarioGrupo values(@usuarioId, @grupoAdmId, 'A')

insert into Usuario values(4, 'wilson@norterefrigeracao.com.br', 'Wilson Teixeira', GETDATE(), 'A', 'S', '9932CD7B406BDFC3F36E399A59428183DADFCFFF', null, null)
select @usuarioId = @@IDENTITY
insert into UsuarioGrupo values(@usuarioId, @grupoAdmId, 'A')

insert into Usuario values(4, 'andreborgesleal@live.com', 'André Borges Leal', GETDATE(), 'A', 'S', 'B65B34214537764876B6133CC2E1F81CE33723BB', null, null)
select @usuarioId = @@IDENTITY
insert into UsuarioGrupo values(@usuarioId, @grupoAdmId, 'A')

declare @transacaoId int

select @transacaoId = Max(transacaoId) + 1from Transacao

insert into Transacao values(@transacaoId, 6, null, 'Registro', 'Formulário de inscrição na promoção', 'Cadastro do cliente', 'Link', 'N', null, 'Account/Index', null)


select * from bolaco..Cliente

delete from UsuarioGrupo where usuarioId >= 1094
delete from Usuario where usuarioId >= 1094
delete from bolaco..Cliente

select * from bolaco..Cliente
select * from Usuario where empresaId = 4
select * from UsuarioGrupo where usuarioId = 1094









commit
-- rollback tran



update bolaco..Parametro set valor = 'S' where paramId = 6

select * from bolaco..Parametro
select * from parc_paradiso..Parametro


select * from seguranca..Usuario where empresaId = 4


insert into seguranca..Empresa values (4, 'Norte Refrigeração', 'bolaaaaco@norterefrigeracao.com.br')


select 'insert into 