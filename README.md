# Plantilla para microservicios en .Net

## Introducción

Esta plantilla esta diseñada para soluciones modernas basadas en microservicios. 

Emplea como base los conceptos de Clean Architecture, Domain Driven Design y CQRS (Mediante MediatR) para el orden del codigo, tomando como objetivo:

* La logica de negocio, en la capa de Dominio (Domain).
* Los requerimientos, que toma como "funcionalidades" (Features), en la capa de Aplicación (Application).
* Las implementaciones de la logica y los requerimientos, en la capa de Infraestructura (Infrastructure).
* La conexión al mundo al mundo exterior, en la capa de WebAPI.

## ¿En que me ayuda esta plantilla?

Provee funcionalidades por defecto para tener codigo que respeta los principios SOLID, a la par que agiliza el desarrollo de software, tales como: 

* Para Domain: 
    * Clases abstractas para AggregateRoots, Entities, ValueObjects y Enumeraciones.
    * Interface base para el IUnitOfWork, dando la oportunidad de usar transacciones con extrema facilidad. 
* Para Application:
    * Una ejecución del codigo a modo de "Pipeline" individual. Esto permite agregar funcionalidades a una Feature en concreto o a varias de ellas de manera sencilla.
* Para Infrastructure:
    * Implementación de UnitOfWork, empleando EntityFramework.
    * Logging y validaciones automaticas para las Features, siendo estas agregadas al "Pipeline" de ejecución.
* Para la WebAPI
    * Interface para hacer una inyección de dependencias más ordenada, desacoplandola del Startup.cs.

### ¿Como usar la plantilla? ###

Para usar la plantilla, debes realizar los siguientes pasos:

1. Agregar la logica de negocio correspondiente a Domain (Ya que es la base de donde se moveran los otros puntos), esto consiste en: 
	1. Definir los contextos delimitados (Bounded Context) (Al ser un microservicio, deberia considerar solo uno, si ves que necesitas varios, quiza deberias revisar las definiciones).
	2. Implementan los AggregateRoots, Entities, ValueObjects y Enumeraciones necesarias, emplea las clases abstractas para esto.
	3. Generar las interfaces de UnitOfWork y Repositorios.
2. Agregar las implementaciones en Infrastructure, esto consiste en:
    1. Implementar el UnitOfWork del Dominio y sus repositorios. 
3. Agregar las funcionalidades en Application
    1. Cada archivo es una funcionalidad autocontenida, y debe incluir una Query (Si es lectura) o un Comando (Si es de mutación). Puede incluir un DTO de respuesta, si corresponde.
3. Agregar los EndPoints correspondientes en WebAPI

Una mención importante, como se menciono antes, este template aplica DDD, por lo que, tambien puede hacerse uso de Eventos de Dominio (Domain Events), 

### Puntos a mejorar

Actualmente, hay varios puntos que se deben pulir y podrian ser mejorados. Se tienen en consideración los siguientes:

* Adaptar más el codigo para como es el desarrollo en Macal (Principalmente, el uso de Database-First en vez de Code-First)
* Agregar ejemplos de como hacer testing: unitario, integración, funcional y de performance.
* Agregar interfaces e implementaciones de repositorios.
* Agregar nuevas funcionalidades al pipeline
* Agregar manejo de eventos de dominio automaticos, que sean independientes del origen de la entidad (Ya que si se usa Database-First, las entidades de Dominio no son las mismas que las de base de datos)

### ¿Comó contribuir?

Puedes contribuir a la plantilla mediante las siguientes acciones:

* Completando algo de "Puntos a mejorar"
* Revisando el codigo
* Mejorando este documento
* Proponiendo nuevas funcionalidades en general
