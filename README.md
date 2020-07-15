# Bolaco
Sistema de Marketing Digital - Bolão dos jogos do Brasil na copa do mundo de 2018 - da empresa Norte Refrigeração

Neste sistema, cada compra realizada pelo cliente na **Norte Refrigeração** (maior empresa de vendas de máquinas e equipamentos de refrigeração do estado do Pará) lhe concede o direito de dar um palpite no resultado dos jogos do Brasil na primeira fase da copa do mundo. Caso o Brasil avance nas demais fases, novos palpites poderão ser cadastrados. 
O cliente também poderá cadastrar quais as duas seleções que farão a grande final da copa.

O cliente realiza seu cadastro no site. Cada compra dá direito a um palpite. Os palpites poderão ser efetuados até uma data limite. Para cadastrar um palpite o cliente precisa informar o número do cupom fiscal e a data da compra que consta no cupom impresso.

Após cadastrar o palpite o sistema envia um e-mail para o cliente com o comprovante da aposta que deverá ser apresentada na loja caso o cliente seja premiado.

Para evitar fraudes, o número do cupom fiscal e a data da compra informadas pelo cliente precisam ser validados no sistema ERP da Norte Refrigeração.

**Como é feita a validação?**

Cada palpite informado e salvo, o sistema grava em uma **fila armazenada em cloud (microsoft Azure)** o número do cupom e a data da compra.

Devido às limitações do ERP que impedem que a atualização ocorra de forma **reativa**, a Norte Refrigeração desenvolveu um JOB que roda de 30 em 30 minutos. Este JOB consome uma API do "Bolaco" que faz a leitura dos cupons armazenados na **fila em cloud no Azure**. O objeto é serializado em formato JSon para ser tratado pelo JOB.

O JOB então valida no ERP os dados e retorna um JSon com o resultado da validação. O JOB consome novamente a API que desserializa o JSon e muda a situação do palpite para confirmado ou cancelado conforme JSon recebido.

Durante os dias que atencedem o jogo do Brasil, o diretor da Norte, com uma senha previlegiada de administrador, pode acompanhar de forma gráfica a evoluação das apostas com os quantitativos de cadastros, quantitativos dos resutados do jogos considerando vitória, empate ou derrota do Brasil.

Esses indicadores servirão de referência para a empresa planejar a distribuição de prêmios, posto que podem haver mais de um ganhador.

No dia do jogo do Brasil, logo após o encerramento da partida, o funcionário de TI da empresa cadastra o resultado do jogo via ferramenta **Postman**, que consome uma API para salvar em banco de dados o resultado e envia uma notificação por SMS e e-mail aos clientes ganhadores. 

Para o diretor é enviado um SMS notificando a quantidade de vencedores.




















