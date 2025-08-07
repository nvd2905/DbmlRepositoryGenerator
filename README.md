# DbmlRepositoryGenerator

A .NET tool for generating Entity Framework repositories and Unit of Work pattern from DBML schema files.

## Features

- Generate Entity Framework entities from DBML schema
- Create repository interfaces and implementations
- Generate Unit of Work pattern
- Support for multiple database providers (SQL Server, MySQL, PostgreSQL, SQLite)
- Customizable templates using Liquid templating engine
- Async/await support
- Command-line interface

## Installation

```bash
dotnet tool install -g DbmlRepositoryGenerator
```

## Usage

### Basic Usage

```bash
dbml-repo-gen -i schema.dbml -o ./Generated -n MyNamespace
```

### Parameters

- `-i, --input`: Path to the DBML schema file (required)
- `-o, --output`: Output directory for generated files (required)
- `-n, --namespace`: Namespace for generated code (default: "Generated")
- `--overwrite`: Overwrite existing files (default: true)

### Example DBML Schema

```dbml
Table User {
  id int [pk, increment]
  name varchar(255) [not null]
  email varchar(255) [not null, unique]
  created_at datetime [not null, default: 'now()']
}

Table Post {
  id int [pk, increment]
  title varchar(255) [not null]
  content text
  user_id int
  created_at datetime [not null, default: 'now()']
}

Ref: Post.user_id > User.id
```

### Generated Output

The tool generates:

- **Entities**: Domain entities with EF Core attributes
- **Repository Interfaces**: Generic repository interfaces
- **Repository Implementations**: EF Core-based repository implementations
- **Unit of Work**: Interface and implementation for transaction management
- **DbContext**: ApplicationDbContext with entity configurations

## Configuration

### Database Providers

Supported database providers:
- SQL Server (default)

### Custom Templates

You can customize the generated code by modifying the templates in the `Templates` directory:

- `entity.liquid`: Entity class template
- `repository-interface.liquid`: Repository interface template
- `repository.liquid`: Repository implementation template
- `unitofwork-interface.liquid`: Unit of Work interface template
- `unitofwork.liquid`: Unit of Work implementation template
- `dbcontext.liquid`: DbContext template

## License

MIT License - see LICENSE file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. 