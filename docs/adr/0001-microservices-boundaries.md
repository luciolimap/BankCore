# 0001 — Fronteiras dos microsserviços

## Status
Aceito

## Contexto
Precisávamos decompor o "núcleo bancário" monolítico em serviços menores. A tentação comum é dividir por camada técnica (ex.: "serviço de API", "serviço de banco"), o que na prática recria um monólito distribuído.

## Decisão
Dividimos por **capacidade de negócio**, seguindo o padrão de bounded contexts:

- **Accounts** — dono do saldo, cadastro e status da conta.
- **Transactions** — dono do registro de depósitos, saques e transferências.

Cada serviço é responsável por sua própria consistência e nunca lê/escreve diretamente no banco do outro.

## Consequências
- Times podem evoluir Accounts e Transactions de forma independente.
- Qualquer alteração de saldo motivada por uma transação passa a ser assíncrona — foi preciso desenhar consistência eventual (ver ADR 0002).
- Sem um "Customer" separado no MVP: o nome do titular vive como campo simples em `Account` para manter o escopo enxuto.
