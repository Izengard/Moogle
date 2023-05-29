__Jocdan Lismar López Mantecón*__
__*Primer año de Lic. en Ciencias de la Computación__
__Facultad de Matemáticas y Computación, Universidad de la Habana__

#      Resumen:
El objetivo de este proyecto es crear un motor de búsqueda de documentos de archivos txt basándose en el Modelo del Espacio Vectorial para la Recuperación de Información. Se realizó una revisión bibliográfica previa de la cual se decidieron los siguientes procedimientos: Cada documento se representa como un vector de sus términos en el Corpus de términos, constituido por todas las palabras de la colección de documentos. Como medida de la importancia de las palabras, se recurrió al factor TF-IDF. Así mismo, se trata la query como un pseudo-documento representado por su propio vector de términos del Corpus. Para la determinación de la similitud entre la query introducida por el usuario y los documentos de la colección se utilizó el indicador Similitud de Cosenos que da una medida de la distancia entre el vector query y los vectores-documento a partir de la amplitud del ángulo que forman estos dos vectores en el espacio vectorial de vectores-documento. Además, se generó un snippet para cada resultado de la búsqueda y se implementaron cuatro operadores de búsqueda que facilitan el control sobre la misma, tambien se implemento un sistema de sugerencias basado en la Distancia de Edicion.
EL proyecto se desarrolló en el Sistema Operativo Windows 10 Pro y fue codificado en el editor de texto Visual Studio Code empleando C# como lenguaje de programación y el framework .NET SDK 7. Se analizaron estructuras de datos factibles para la ejecución del mismo donde resaltan, además de los conocidos array<T>, las colecciones List<T> y HashSet<T> destacándose este último por su elevado rendimiento temporal. También se estudió el tipo System.String y sus métodos para la generación de Snippets y las expresiones lambda.
Objetivos: 
•	Crear un motor de búsqueda de documentos en formato .txt empleando el Modelo de Espacios Vectoriales para la Recuperación de Información.
•	Generar un snippet para cada resultado de la búsqueda.
•	Implementar funcionalidades adicionales que mejoren la calidad de la búsqueda y el control sobre la misma.


# Ejecutando el proyecto:____________________________________________
1)	Asegúrese de tener instalado .NET Core 7. 
2)	Ir al directorio del proyecto “Moogle project”.
3)	Copiar la base de datos (colección de documentos en formato .txt) a la carpeta Content
4)	Ejecutar en la terminal de Linux:
```bash
make dev
```
Si estás en Windows, desde la carpeta raíz del proyecto:
```bash
dotnet watch run --project MoogleServer
```


