use bolaco
go

SELECT     1 AS [C1],     'Bandeira 1' AS [C2],     'Brasil' AS [C3],     [GroupBy1].[K1] AS [score1Brasil],     'Bandeira 2' AS [C4],     'Croácia' AS [C5],     [GroupBy1].[K2] AS [score1Croacia],     [GroupBy1].[A1] AS [C6],     9 AS [C7]    FROM ( SELECT         [Extent1].[score1Brasil] AS [K1],         [Extent1].[score1Croacia] AS [K2],         COUNT(1) AS [A1]        FROM [dbo].[Ticket] AS [Extent1]        WHERE [Extent1].[score1Brasil] IS NOT NULL        GROUP BY [Extent1].[score1Brasil], [Extent1].[score1Croacia]    )  AS [GroupBy1]



select * from selecao


update parametro set valor = 'N' where paramId = 6

select * from Parametro

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




use seguranca
go


delete from seguranca..Sessao where sistemaId = 6
delete from UsuarioGrupo where usuarioId >= 1100
delete from Usuario where usuarioId >= 1100
delete from bolaco..Ticket
delete from bolaco..Cliente


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











