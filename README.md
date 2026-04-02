# Totem Atendimento

Sistema de autoatendimento para o serviços hospitalares.
Desenvolvido em C# WinForms (.NET 8).

## Estrutura do Projeto

```
TotemSantaCasa/
├── Config/
│   └── Configuracoes.cs         # Leitura do appsettings.json
├── Forms/
│   ├── UIHelper.cs              # Componentes visuais reutilizáveis
│   ├── FormInicial.cs           # Tela principal: Consultas / Exames / Senhas
│   ├── FormDigitarCPF.cs        # Teclado numérico para digitação do CPF
│   ├── FormCPFNaoEncontrado.cs  # Exibida quando CPF não tem agendamento
│   ├── FormConfirmarTelefone.cs # Confirmação dos 4 últimos dígitos do tel.
│   ├── FormConfirmarDados.cs    # Exibe dados do paciente para conferência
│   ├── FormSenhas.cs            # Grade de tipos de senha (Check-in Térreo etc.)
│   └── FormConclusao.cs         # Tela de sucesso + FormConclusaoSenha + FormLoading
├── Models/
│   └── Modelos.cs               # Paciente, Agendamento, ResultadoCheckin, enums
├── Printing/
│   └── ImpressaoService.cs      # Impressão ZPL (Zebra) e GDI+ (térmica)
├── Services/
│   └── ApiService.cs            # Integração HTTP com o sistema Tasy (+ mock)
├── Program.cs                   # Entry point com loop de reinício automático
└── appsettings.json             # Configuração de impressoras, API e totem
```

## Fluxo de Atendimento

```
Tela Inicial
  ├── CONSULTAS / EXAMES
  │     └── Digitar CPF
  │           ├── CPF não encontrado → Aviso → Tela Inicial
  │           └── CPF encontrado
  │                 └── Confirmar Dados
  │                       ├── Dados incorretos → Guichê
  │                       └── Confirmar
  │                             ├── Integração Tasy
  │                             ├── Imprimir pulseira (Zebra ZPL)
  │                             ├── Imprimir comprovante (térmica GDI+)
  │                             └── Tela Conclusão → Auto-retorno
  └── SENHAS
        └── Selecionar tipo
              ├── Imprimir senha (térmica GDI+)
              └── Tela Conclusão Senha → Auto-retorno
```

## Configuração

### appsettings.json

```json
{
  "Impressoras": {
    "ImpressoraTermica": "NOME_DA_IMPRESSORA_TERMICA",
    "ImpressoraZebra": "NOME_DA_IMPRESSORA_ZEBRA"
  },
  "API": {
    "BaseUrl": "https://sua-api.com",
    "Timeout": 30,
    "ChaveAcesso": "SUA_CHAVE"
  },
  "Totem": {
    "NomeInstituicao": "Santa Casa de Curitiba",
    "Ambulatorio": "Dom Eurico",
    "TimeoutTela": 60
  }
}
```

> O nome da impressora deve ser exatamente igual ao nome configurado no Windows
> (Painel de Controle > Dispositivos e Impressoras).

### Modo Mock (sem API)
Se `API.BaseUrl` estiver vazio, o sistema usa dados fictícios para desenvolvimento.
Para simular CPF não encontrado, use CPF `00000000000`.

## Impressoras

### Zebra (Pulseira de Identificação)
- Protocolo: ZPL II enviado via RAW (WinSpool)
- Conteúdo: Nome, nascimento, sexo, mãe, atendimento, setor, médico, código de barras
- Modelos suportados: ZD220, ZD230, ZD420 e demais com suporte ZPL

### Térmica (Comprovante / Senha)
- Protocolo: GDI+ via `PrintDocument`
- Conteúdo comprovante: dados completos do atendimento
- Conteúdo senha: número grande centralizado + tipo de atendimento

## Requisitos

- .NET 8 SDK (Windows)
- Visual Studio 2022 ou superior
- Windows 10/11
- Impressoras instaladas e compartilhadas no Windows

## Build

```bash
dotnet restore
dotnet build
dotnet run --project TotemSantaCasa/TotemSantaCasa.csproj
```

## Próximos Passos (Integração API)

O arquivo `Services/ApiService.cs` contém stubs prontos para implementar:

- `BuscarPacientePorCpfAsync(cpf)` → `GET /pacientes/{cpf}`
- `RealizarCheckinAsync(cpf, tipo)` → `POST /checkin`
- `ConfirmarAtendimentoAsync(numero)` → `POST /atendimentos/{numero}/confirmar`

Autenticação, mapeamento de DTOs e tratamento de erros específicos do Tasy
devem ser adicionados conforme a documentação da API disponibilizada pela equipe.
