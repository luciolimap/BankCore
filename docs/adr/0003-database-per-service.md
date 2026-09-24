# 0003 — Um banco de dados por serviço

## Status
Aceito

## Contexto
Um banco compartilhado entre Accounts e Transactions permitiria queries diretas entre os domínios, mas recriaria o acoplamento típico do monólito legado — qualquer mudança de schema em um lado arriscaria quebrar o outro.

## Decisão
Cada serviço tem seu próprio banco lógico (`BankCore.Accounts`, `BankCore.Transactions`), sem foreign keys entre eles. Toda referência entre os domínios (ex.: `SourceAccountId` em `Transaction`) é apenas um identificador opaco, sem integridade referencial garantida pelo banco — a consistência é garantida pelo fluxo assíncrono de eventos (ADR 0002).

Em produção/Docker, ambos usam SQL Server. Para desenvolvimento local sem Docker, cada serviço cai para SQLite (mesma separação, banco de arquivo por serviço).

## Consequências
- Nenhuma query cruza os dois domínios diretamente; qualquer necessidade de dado agregado exigiria uma composição no nível de aplicação (ex.: BFF) ou um serviço de leitura dedicado — não implementado no MVP.
- Migrations de cada serviço evoluem de forma independente.
