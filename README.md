# AlterData
Projeto para atender ao gerenciamento de produtos e clientes empresariais, usando tecnologias como C#, Vue.js, EF-8.

# Descritivo
Neste repositório teremos a aplicação mobile no formato SPA (Single Page Application), que irá consumir os recursos ofertados por uma web api.

O app será criado utilizando Vue.JS, sendo necessário a autenticação válida para acesso aos dados ofertados pelo WS.

A API será desenvolvida no padrão RESTful, na versão 8 do AspNet Core.

Usaremos o banco de dados PostgreSQL na versão 17.2 (21 novembro 2024). Para persistência e manipulação de dados será usado o ORM Entity Framework 8.

# Planejamento
Iniciaremos pela criação de entidades de domínio que serão persistidas ao banco de dados. Isso faremos em um projeto de classes, separado do projeto principal, dessa forma poderemos mudar as entidades de domínio sem afetar a API.

Em seguida vamos criar as classes de contexto de banco, serviços e repositório.

Após essas etapas iremos iniciar os testes, mockando e criando métodos para validar os parâmetros enviados para as entidades de domínio através da API, para garantir que respeitam as regras de negócio e de atributos.

Iremos criar documentação da API usando Swagger.

Ao final iremos criar o app SPA usando Vue.JS, seguindo as regras estabelecidas.

Demais informações e detalhes do projeto olhar a pasta "Documentação" na raiz do repositório.



