# Simur Pagamentos
**Projeto de estudo que implementa um simulador de pagamentos**

O Simur visa simular uma plataforma de pagamentos, criando, recebendo e processando pagamentos até seus devidos fins.


## TECNOLOGIAS
- **Backend**: ASP.NET 10
- **Segurança**: Autenticação JWT com Roles
- **Banco de dados**: MongoDB com Replica Set
- **Documentação**: Scalar com OpenAPI
- **Infraestrutura**: Docker e Docker Compose
- **Mensageria**: RabbitMQ com DLQ
- **Serviço externo**: NodeJS


## ARQUITETURA GERAL
### simur-backend
Esse representa o núcleo do projeto, composto pelos serviços e pelo processador de pagamentos.

1. **Serviços**:
- Authentication: implementação de JWT e Roles para autenticação e controle de acesso
- UserServices: criação e autenticação de usuários
- MerchantServices: cadastro de entidades com perfil de beneficiárias de pagamentos
- CustomerServices: cadastro de entidades com perfil de pagantes
- PaymentServices: núcleo do projeto com CRUD completo de pagamentos, incluindo atualização de status

2. **Entidades de Pagamentos**:
- Payment: entidade central que além dos atributos principais de pagamento, relaciona Merchant, Customer e External Order ID para referenciar o pedido gerador do pagamento
- PaymentMethod: especifica o tipo de pagamento mais o seu detalhamento
- IPaymentDetails: interface para implementação de polimorfismo para diferentes tipos de pagamento
- BoletoDetails: recebe os dados de pagamento (pagador, beneficiário, valor, banco) e gera o código de barras e linha digitáveis no padrão FEBRABAN
- CreditCardDetails: recebe os dados do pedido, parcelas e o token do cartão para registrar um pagamento a prazo
- DebitCardDetails: recebe os dados do pedido e o token do cartão para registrar um pagamento à vista
- PixDynamicDetails: recebe apenas os dados do pedido e gera uma URL de pagamento com CRC e QR Code de acordo com o padrão de PIX dinâmico pelo Bacen
- PixStaticDetails: recebe apenas os dados do pedido e gera uma URL de pagamento com CRC e QR Code de acordo com o padrão de PIX estático pelo Bacen
- PaymentStatusHistory: relaciona cada pagamento a um histórico de ciclo de vida dedicado, persistindo as mudanças situacionais de casa pagamento ao longo do tempo

3. **Entidades de usuários e clientes**:
- Customer: eventuais clientes que se cadastrem na plataforma a fim de recuperar históricos de pagamentos (TBD)
- Merchant: empresas e prestadores que queiram utilizar os serviços de pagamentos do Simur com seus clientes
- User: usuários de sistema para credenciamento e autenticação, tanto podem ser Customers e Merchants como perfis com privilégios específicos (administradores, gerentes, etc)
- Address: entidade para padronizar e complementar cadastros de Customers e Merchants com seus respectivos endereços

4. **Mensageria**:
- RabbitMqSetupService: classe centralizadora para iniciar e conectar-se ao RabbitMQ
- RabbitMqPublisherService: contém o método responsável por enviar à fila o status tanto na criação como na atualização de pagamentos
- RabbitMqConsumerService: implementa um listener que consome da fila de status de pagamentos atualizados por um acquirer

5. **Exceções**:
- SimurExceptionHandler: middleware centralizado para tratamento padrão de exceções no projeto


### global-acquirer
Trata-se de uma aplicação NodeJS simples criada para ser uma simulação de um acquirer. Seu papel é de consumir da fila os pagamentos gerados pelo Simur e devolvê-los com status atualizado, incluindo margem para retornos de negativa (rejeitado, bloqueado, etc)
- ManageBankSlip: implementa a lógica para tramitação de pagamentos por boleto, incluindo verificação de vencimento para rejeição
- ManageCard: implementa a lógica para tramitação de pagamentos por débito e crédito, incluindo uma margem percentual para simular falha
- ManagePix: implementa a lógica para tramitação de pagamentos por PIX, incluindo uma margem percentual para simular falha e verificação de vencimento para rejeição
- GlobalAcquirerConsumer: implementa o listener responsável por consumir da fila, tratar o status de pagamento e publicar a atualização


## ESTRUTURA DO PROJETO
```
simur-backend
│           
├───Auth
│     ├── TokenConfiguration
│     └── TokenGenerator
│───Configurations
│───Controllers
│     ├── Authorization
│     ├── Gateway
│     └── Utils
│───Exceptions
│     └── CustomExceptions
│───Hypermedia
│     ├── Enrichers
│     └── Filters
│───Mappers
│───Messaging
│───Models
│     ├── Deserealizers
│     ├── DTO
│     └── Entities
│───Repositories
│───Services
└───Utilities
```

## FUNCIONAMENTO
A execução deste projeto não se dá sem a integração de todos os componentes de seu ambiente.
Para tal, é necessário utilizar a infraestrutura conforme definida no arquivo docker-compose

### PRÉ-REQUISITOS
- Microsoft .NET SDK 10
- Docker & Docker Compose

### VARIÁVEIS DE AMBIENTE
Tanto o docker-compose como as aplicações utilizam um arquivo _.env_ para centralização e encapsulamento de dados sensíveis e parâmetros cuja definição pode mudar a depender do ambiente.
<br>O arquivo _ExampleEnvironmentVariables.txt_ contém os nomes das variáveis e os nomes das filas para apoio.
<br>Assim, é importante que haja um arquivo _.env_ nos seguintes diretórios:
1. Na raiz da solução, conjuntamente com o docker-compose
2. Dentro do diretório _simur-backend_
3. Dentro do diretório _global-acquirer_

### ESTRUTURA DO DOCKER-COMPOSE
1. MongoDB
- **mongo-setup**: fará a inicialização do banco com definição de um replica-set utilizando o script _init-mongo-replicaset.sh_
- **mongo1**: replica-set do banco de dados que ficará disponível para uso pelas aplicações
2. RabbitMQ
- **broker**: serviço de mensageria escolhido para o projeto que precisará de um usuário e uma senha disponibilizados como variáveis de ambiente
3. Global Acquirer
- **global-acquirer**: a execução dessa aplicação NodeJS deve ser realizada após a do RabbitMQ pelo comando ```npm start```, com o arquivo _.env_ no diretório raiz contendo as variáveis de ambiente que especificarão a conexão com o broker.


### EXECUÇÃO LOCAL
1. Prepare os arquivos _.env_ ou configure as variáveis de sistema conforme o modelo apresentado no arquivo de referência _ExampleEnvironmentVariables.txt_
2. Execute o docker-compose ```docker-compose up```
3. Aguarde os serviços ficarem todos online, sendo o global-acquirer o último a inicializar
4. Execute o simur-backend com o comando ```dotnet run```
5. Acesse a URL https://localhost:7033 para ser redirecionado ao Scalar onde as APIs poderão ser consultadas


### FLUXO DE MENSAGERIA
```
[simur-backend]                                                     [global-acquirer]
       ↓
Cria um pagamento
       ↓
Publica status          →        [RabbitMQ: ready.payments]     →   Consome pagamento criado
                                                                                ↓
                                                                    Atualiza status do pagamento
                                                                                ↓
Consome e atualiza BD   ←       [RabbitMQ: updated.payments]    ←   publica status atualizado

```

### LICENÇA
    Feito com ☕ e persistência por Carlos Eduardo de Souza Viana
    LinkedIn: https://www.linkedin.com/in/carlos-eds-viana/