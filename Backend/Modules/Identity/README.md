# Identity module

    ## Owns

    - `Users`
- `RefreshTokens`
- `PasswordResetTokens`
- `EmailVerificationTokens`

    ## Internal structure

    ```text
    Identity/
    ├── Controllers
    ├── Services
    ├── Repositories
    ├── Interfaces
    ├── Entities
    ├── DTOs
    ├── Validators
    └── IdentityModule.cs
    ```

    Controllers receive HTTP requests. Services enforce business rules.
    Repositories perform database access. DTOs define API contracts.
    Validators validate incoming DTOs.

    Do not access another module's repository directly. Depend on an
    interface/service exposed by that module.
