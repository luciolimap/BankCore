# 0002 — Mensageria assíncrona com MassTransit

## Status
Aceito

## Contexto
Accounts e Transactions precisam se comunicar sem acoplamento síncrono direto (evitando que uma chamada HTTP travada em um serviço derrube o outro) e sem compartilhar banco de dados.

## Decisão
Usamos **MassTransit 8** sobre **RabbitMQ**, em vez de:
- Chamadas HTTP síncronas entre os serviços (acoplaria disponibilidade dos dois).
- Cliente `RabbitMQ.Client` puro (exigiria implementar manualmente topologia, retry, serialização e DI).

MassTransit cria a topologia (exchanges/filas) automaticamente a partir dos tipos de evento, tem integração nativa com `IServiceCollection`, e principalmente oferece `MassTransit.Testing` com um harness de teste que sobe um bus in-memory dentro do processo de teste — permitindo testar consumers publish/consume sem depender de um RabbitMQ real.

O fluxo é uma **mini-saga coreografada** (sem orquestrador central): Transactions publica `TransactionRegistered`; Accounts aplica o débito/crédito e responde com `TransactionApproved` ou `TransactionRejected`; Transactions atualiza o status final.

## Consequências
- Em ambiente sem Docker, o transporte in-memory do MassTransit só funciona dentro de um único processo — não conecta dois serviços rodando separadamente (documentado no README).
- Sem orquestrador central, a lógica de compensação fica distribuída entre os dois serviços — aceitável na escala atual (2 serviços, 1 saga simples), mas não escalaria para sagas com muitos passos sem um orquestrador (ex.: MassTransit Saga State Machine).
