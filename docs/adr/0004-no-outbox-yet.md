# 0004 — Sem Outbox Pattern (ainda)

## Status
Aceito como débito técnico conhecido

## Contexto
Em `TransactionService.RegisterAsync`, salvar a transação no banco e publicar o evento `TransactionRegistered` são duas operações separadas (uma transacional no banco relacional, outra no broker). Se o processo cair entre as duas, a transação fica persistida mas o evento nunca é publicado — um "dual write" clássico.

O padrão consolidado para resolver isso é o **Transactional Outbox**: gravar o evento na mesma transação do banco, em uma tabela de outbox, e um publicador separado (ou o próprio MassTransit, que tem suporte nativo a outbox via EF Core) o envia de forma garantida.

## Decisão
Não implementamos outbox no MVP. É uma lacuna conhecida e documentada, não um ponto cego.

## Consequências
- Risco real, mas de baixa probabilidade na demo (janela de falha muito pequena entre `SaveChangesAsync` e `Publish`).
- Próximo passo natural do roadmap: habilitar `UsingInMemoryOutbox` ou o outbox baseado em EF Core do MassTransit (`AddEntityFrameworkOutbox`), que é majoritariamente configuração, não reescrita.
