## Chat Teste

O chat possui as seguintes funcionalidades:
  - Mensagem pública para a sala ( [mensagem] ou /m [mensage] )
  - Mensagem pública para um usuário  ( /t [nome do usuário] /m [mensagem] )
  - Mensagem privada para um usuário ( /p [nome do usuário] /m [mensagem] )
  - Criação de salas ( /cr [nome da sala] )
  - Saidas de salas ( /exit [nome da sala] )
  - Troca de salas, para outras salas públicas ou privadas em que o usuário esteja incluído ( /ch [nome da sala]) 
  - Comando de ajuda( /h )
  - Listagem de usuário online ( /lu )
  - Listagem de salas públicas e privadas em que o usuário esteja incluído ( /lr )

A funcionalidade de mensagem privada, troca o usuario que enviou para a sala privada forçadamente, o recepitor será notificado que recebeu uma mensagem em uma outra sala.
Os usuário são notificadas caso recebam mensagem em outras salas que esteja incluídos, mesmo que não esteja nela no momento, para vizualizar as mensagens de outra sala, o usuário deverá trocar para a sala em questão.
Quando o ultimo usuario da sala sair, ela será deletada.

#### Bug conhecido: se um usuario não entrou em uma sala cridada pelo envido de mensagem direta, ela não aparecerá na listagem das sala.
#### Bug conhecido: no projeto Client, esta com um problema de concorrencia ao acesar e atualizar as salas de usuário, será necessário alterar o modo de notidicação de atualização das salas, como não tenho tempo para resolver, irei enviar o projeto assim mesmo.

O projeto foi desenvolvido como uma Web API Core, tanto o Client quanto o Server.
A porta da url do Client é gerada randomicamente ao iniciar a aplicação, existe a possibilidade de conflitos com outras aplicação, inclusive com outras instâncias do próprio Client, se isso ocorrer, reinicie uma das instâcias do Client;
Não a possibilidade de conflito de URL entre os Clients e o Server.

Será possível encontrar algum resquicio de codigo criado para teste no codigo, o que irá suja ele um pouco. Tentei deixar o codigo mais arrumado possivel, mas com o passar do tempo, fiquei com o tempo apertado, e me descuidei um pouco. 

Os comandos foi implementando usando o padrão Strategy, basicamente foi o único padrão de projeto usado nesse projeto.
Tentei isolar completamente a parte responsável pela inteface no Client, não me preocupei muito com a gerencia do texto impresso no console, então será normal ver formatação estranha, mas tentei pelo menos deixar as saídas claras e fáceis de ler.
Separei as classes, fazendo o possível para que todos metodos tivesse uma caracteristica em comum, por exemplo, classe responsável pela comunicação com servidor.

Nos testes será possível observar repetição de código, parte disso se dá, pela possibilidade de mudança de comportamento para a situação testada no pelo teste, e parte se dá a presa ao escrever os testes, pois muitos deles foram escritos durante o último dia.
Os testes tem uma cobertura bem limitada, o motivo é o mesmo dito anteriomente.

Apontei a maioria das coisas que queria aqui, apesar de parecer que dei foco nos problemas, minha inteção é esclarecer que tenho ciência deles, o que me falta é tempo. Os três projetos, necssários para execução dos teste estão funcionando, e para um teste simples, basta abrir duas instancias do Client e uma do Server

## Atenção
### Os software foram feitos somente para rodar em ambiente local.
#### - Não foi implementado nenhum tipo de mecanismo para seguraça das mensagens
#### - Não foi implementado nenhum metodo de persistencia das informações, todos os dados são salvi em memória.

## Rodar

Os projetos foram escritos em .NET Core 5, então é necessário que tenha o SDK dessa versão instalado para realização do build dos projetos no Visual Studio.
Após os builds, basta ir nas pastas bin dos projetos e iniciá-los, comece pelo servidor (../Chat.Server/bin/net5.0) e depois abra quantas instâncias quiser do cliente (../Chat.Client/bin/net5.0). Lembrando que não foram realizados testes com no maximo três instâncias do cliente. 
Ao iniciar uma instância do cliente será necessário escolher um nome, e após isso, já é possível trocar mensagens com os outros clientes.
