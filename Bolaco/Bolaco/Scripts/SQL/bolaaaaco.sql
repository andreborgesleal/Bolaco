use bolaco
go

alter table ticket
add dt_avaliacao datetime null, ind_avaliacao char(1) null

select * from Ticket


select * from bolaco..Ticket
order by dt_inscricao desc

update bolaco..Selecao set nome = 'Bósnia' where selecaoId = 19
select * from bolaco..Selecao
select * from selecao
select * from bolaco..Cliente order by dt_cadastro desc

update bolaco..parametro set valor = 'N' where paramId = 6

select * from bolaco..Parametro

update Parametro set valor = '06/05/2014' where paramId = 8

insert into Parametro values(8, 'Data Inicial da promoção', 'Data início da campanha', 'L', '12/05/2014')
insert into Parametro values(9, 'Data final da promoção', 'Data final da campanha', 'L', '07/07/2014')
insert into Parametro values(10, 'score1Brasil', 'Score do Brasil', 'L', '')
insert into Parametro values(11, 'score1Croacia', 'Score Croacia', 'L', '')
insert into Parametro values(12, 'score2Brasil', 'Score Brasil', 'L', '')
insert into Parametro values(13, 'score2Mexico', 'Score México', 'L', '')
insert into Parametro values(14, 'score3Brasil', 'Score Brasil', 'L', '')
insert into Parametro values(15, 'score3Camaroes', 'Score Camaroes', 'L', '')
insert into Parametro values(16, 'score1_final', 'Score da final seleção 1', 'L', '')
insert into Parametro values(17, 'score2_fiinal', 'Score da final seleção 2', 'L', '')
insert into Parametro values(18, 'selecao1_final', 'Seleção 1 Final', 'L', '')
insert into Parametro values(19, 'selecao2_final', 'Seleção 2 Final', 'L', '')


update seguranca..Sessao set dt_desativacao = getdate() where empresaId = 4

select * From seguranca..LogAuditoria where empresaId = 4
select * from seguranca..Sessao where empresaId = 4 -- xmje30ncnq0ue2e2syaq4mha
select * from seguranca..Sistema
select * from seguranca..Empresa

select * from seguranca..Usuario where usuarioId >= 1127
select * from seguranca..UsuarioGrupo where grupoId in (select grupoId from seguranca..Grupo where seguranca..Grupo.sistemaId = 6)
select * From seguranca..Grupo where grupoId in (1009, 1010)
select * From seguranca..GrupoTransacao where grupoId = 1010
select * From seguranca..Transacao where sistemaId = 6
select * from seguranca..GrupoTransacao where transacaoId in ( 415, 416)


update seguranca..Transacao set url = 'Home/index' , nome = 'Formulário de cadastro do palpite', descricao = 'Formulário de cadastro do palpite', nomeCurto = 'Palpite'
where transacaoId = 415

insert into seguranca..GrupoTransacao(grupoId, transacaoId, situacao) values(1010, 415, 'A')

declare @transacaoId int
select @transacaoId = max(transacaoId) + 1 from seguranca..Transacao

insert into seguranca..Transacao(transacaoId, sistemaId, nomeCurto, nome, descricao, referencia, exibir, url) 
values(@transacaoId, 6, 'Registre-se', 'Formulário de inscrição do cliente','Formulário de inscrição do cliente', 'Link', 'N', 'Account/Index')


use seguranca
go


delete from seguranca..Sessao where sistemaId = 6
delete from seguranca..UsuarioGrupo where usuarioId = 1136
delete from seguranca..Usuario where usuarioId = 1136 and empresaId = 4
delete from bolaco..Ticket where clienteId >= 13 
delete from bolaco..Cliente where clienteId >= 32

select * from bolaco..Cliente where usuarioId = 1109 

select * from seguranca..Usuario where empresaId = 4
select * from seguranca..UsuarioGrupo where usuarioId = 1135

select * from seguranca..Sessao where sistemaId = 6
select * from bolaco..Cliente
select * from Usuario where empresaId = 4
select * from UsuarioGrupo where usuarioId >= 1100











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


commit
-- rollback tran



update bolaco..Parametro set valor = 'S' where paramId = 6

INSERT into bolaco..Parametro values(7, 'Fuso Horário', 'Tempo a ser adicionado ou dminuído na hora de gravar a data e hora do palpite', 'L', '-3')

select * from bolaco..Parametro
select * from parc_paradiso..Parametro


select * from seguranca..Usuario where empresaId = 4


insert into seguranca..Empresa values (4, 'Norte Refrigeração', 'bolaaaaco@norterefrigeracao.com.br')



select * from ticket
delete from ticket where score3Camaroes is null


use bolaco
go

select * from selecao


insert into selecao values('Brasil', '')
insert into selecao values('Croácia', '')
insert into selecao values('Camarões', '')
insert into selecao values('México', '')
insert into selecao values('Alemanha', '')
insert into selecao values('Inglaterra', '')
insert into selecao values('Itália', '')
insert into selecao values('Espanha', '')
insert into selecao values('Argentina', '')
insert into selecao values('Uruaguai', '')
insert into selecao values('Holanda', '')
insert into selecao values('Chile', '')
insert into selecao values('Austrália', '')
insert into selecao values('Colômbia', '')
insert into selecao values('Grécia', '')
insert into selecao values('Costa do Marfim', '')
insert into selecao values('Japão', '')
insert into selecao values('Costa Rica', '')
insert into selecao values('Bósnia Herzegovina', 'bandeira-bosnia-herzegovina')
insert into selecao values('Iran', '')
insert into selecao values('Nigéria', '')
insert into selecao values('Suíca', '')
insert into selecao values('Equador', '')
insert into selecao values('França', '')
insert into selecao values('Honduras', '')
insert into selecao values('Portugal', '')
insert into selecao values('Gana', '')
insert into selecao values('Bélgica', '')
insert into selecao values('Argêlia', '')
insert into selecao values('Rússia', '')
insert into selecao values('Coréia do Sul', '')
insert into selecao values('Estados Unidos', '')











