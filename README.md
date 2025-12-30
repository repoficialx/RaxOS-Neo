# RaxOS Neo (v0.1)

RaxOS Neo es un proyecto experimental de sistema operativo desarrollado en C#. Esta versión inicial (v0.1) es una prueba de concepto orientada a investigación y aprendizaje sobre diseño de sistemas operativos usando el ecosistema .NET/C#.

Estado
- Versión: v0.1
- Estado: Desarrollo inicial / Prueba de concepto
- Lenguaje principal: C# (100%)

Objetivo
- Explorar conceptos de sistemas operativos (kernel, shell, manejo básico de drivers, modularidad) aplicados con C# y .NET.
- Servir como repositorio educativo y base para experimentos y contribuciones.

Características (actuales / planeadas)
- Estructura modular del código para facilitar experimentos y contribuciones.
- Núcleo mínimo y utilidades básicas (en desarrollo).
- Shell y comandos mínimos (en progreso).
- Soporte previsto para módulos o controladores (planificado).
- Documentación y ejemplos para desarrolladores que quieran contribuir.

Requisitos
- .NET SDK (.NET 6)
- Windows 10 o superior con Visual Studio 2022 o superior
- Herramientas opcionales: VMware y la extensión de Cosmos para VS (si el flujo de trabajo requiere emulación o imágenes de arranque)

Instalación y compilación (guía rápida)
1. Clona el repositorio:
   git clone https://github.com/repoficialx/RaxOS-Neo.git

2. Entra en la carpeta del proyecto (ajusta la ruta si corresponde):
   cd "RaxOS Neo"

3. Compila el proyecto:
   dotnet build

   Si existe una solución o proyecto específico, usa:
   dotnet build ./ruta/al/Proyecto.sln
   o
   dotnet build ./ruta/Proyecto.csproj

4. Ejecuta usando F5 en el IDE o añadiendo la ISO a una máquina virtual en tu software de emulación preferido.

Cómo contribuir
- Abre un issue para proponer una mejora o reportar un bug.
- Crea pull requests pequeños y enfocados. Incluye descripción clara y pasos para reproducir cambios o errores.
- Añade pruebas o ejemplos cuando sea posible.
- Sigue las buenas prácticas: commits claros, ramas por característica, y documentación de cambios.

Buenas prácticas para PRs
- Título claro y relación con el issue si aplica.
- Descripción del problema y la solución.
- Instrucciones para probar los cambios.
- Si el cambio afecta la API o comportamiento, actualiza la documentación.

Licencia
Este proyecto posee la licencia MIT ©2024-26

Contacto
- Autor / Mantenedor: repoficialx
- Repositorio: https://github.com/repoficialx/RaxOS-Neo

Registro de cambios (Changelog)
- v0.1 — Inicial: estructura básica del repositorio y documentación mínima.
