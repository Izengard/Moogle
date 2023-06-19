# Moogle!

![](moogle.png)

> Proyecto de Programación I.
> Facultad de Matemática y Computación - Universidad de La Habana.
> Cursos 2021, 2022, 2023.

Moogle! es una aplicación *totalmente original* cuyo propósito es buscar inteligentemente un texto en un conjunto de documentos.

Es una aplicación web, desarrollada con tecnología .NET Core 7.0, específicamente usando Blazor como *framework* web para la interfaz gráfica, y en el lenguaje C#.
La aplicación está dividida en dos componentes fundamentales:

- `MoogleServer` es un servidor web que renderiza la interfaz gráfica y sirve los resultados.
- `MoogleEngine` es una biblioteca de clases donde está implementada la lógica del algoritmo de búsqueda.

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

# Introducción: ____________________________________________#
Modelo de Espacio Vectorial para la Recuperación de Información (MEV):
El modelo de espacio vectorial es una técnica estándar en la los Sistemas de Recuperación de Información, consiste en representar los documentos como vectores de las palabras que los constituyen. Se construye un vocabulario con todas las palabras de la colección de documentos de forma que no se repitan los términos y cada uno de estos términos representa una dimensión del espacio. Los vectores-documento representaran la importancia de cada término del vocabulario en el propio documento mediante su valor de peso, así mismo la query se trata como un pseudo-documento y se representa mediante un vector en el espacio. Luego, se emplean indicadores de similitud para determinar que documentos son similares a otros o a una query dada. 
# Term Frequency – Inverse Document Frequency (TF-IDF):
Esta medida de peso de los términos se basa en dos medidas estadísticas:
	Frecuencia de términos (TF): mide la frecuencia relativa con la que un término aparece en un documento, es decir, cuantas veces aparece el término en el documento (frecuencia absoluta Nw) dividido por la cantidad total (Td) de palabras del documento. La fórmula utilizada es:
〖TF〗_w  =  N_w/T_d   
La intuición con esta medida es que los términos que aparecen más veces probablemente sean más importantes para el contenido del documento.
	Frecuencia inversa de documento (IDF): Mide la importancia del término en la colección de documentos, es decir, cuantos documentos de la colección contienen el término.
Se empleó la fórmula:
〖IDF〗_(w )=〖log〗_2 ((|D|)/(|d_w |)  )
Dónde |D| es la cardinalidad de la colección de documentos, es decir, la cantidad total de documentos en la colección y |dw| es la cantidad de documentos de la colección que contienen el término. El logaritmo se utiliza para evitar asignar demasiado peso a los documentos que aparecen en muchos documentos y para que los términos que aparecen en pocos documentos tengan un mayor peso. 
La idea que subyace esta medida es que los términos que aparecen en muchos documentos probablemente sean menos importantes y los que aparecen en menos documentos son más específicos y más relevantes para la búsqueda.

La medida TF-IDF asigna un peso a cada término en cada documento de la colección. El peso de un término se calcula multiplicando la frecuencia del término TF por la frecuencia inversa del documento IDF:

〖weight〗_w  =〖TF〗_w  × 〖IDF〗_w

Una vez que se han calculado los pesos para cada término en cada documento de la colección se pueden utilizar para calcular la similitud entre una query y los documentos de la colección. En este caso se representan los documentos como un vector de pesos y se calcula la similitud de cosenos.

# Similitud de Cosenos:
Es una técnica utilizada en la Recuperación de Información para comparar la similitud entre dos vectores, se basa en el ángulo entre los mismos. Cuanto menor sea el ángulo, mayor será la similitud entre los vectores. La similitud de cosenos se calcula como el coseno del ángulo entre los vectores mediante la fórmula:

sim =(A*B)/(||A||*||B||)

Donde A y B son los vectores que se están comparando, * representa el producto punto de los vectores, y ||A||, ||B|| representan las magnitudes de los mismos.
El producto punto entre dos vectores es la suma de los productos de los componentes correspondientes de los vectores. La magnitud de un vector es la raíz cuadrada de la suma de los cuadrados de sus componentes. La similitud de cosenos se puede calcular directamente a partir de los pesos de los términos en los vectores.

A*B= ∑▒〖〖(a〗_i*〗 b_i)              |A|= √(∑▒a_i^2 )
# Generando el snippet:
Para generar del snippet se decidió mostrar un subtexto del documento que contenga la palabra de la query de mayor importancia (mayor TF-IDF). b Se extrae inicialmente un snippet con un tamaño de 80 caracteres alrededor de la palabra mas importante de la query con respecto al documento y al corpus; a partir de este primer resultado seguimos tomando caracteres hasta llegar al comienzo y al final de la oración completa respectivamente buscando la primera aparición de los signos de puntuación más comunes que pueden delimitar una oración: “ ¿?  .   ;  ¡!  ”

# Operadores de Búsqueda:
Los operadores de búsqueda añaden opciones y control sobre los resultados de la búsqueda. Se escriben inmediatamente antes de la palabra sobre la cual se quiere aplicar el operador.
Para su implementación se calcula un factor al que se nombró scoreModifier (sM) el cual es un modificador porcentual que multiplicará al valor de Similitud de Cosenos entre el vector documento y el vector query obteniéndose el resultado de Score final. Cada operador contribuye (si aparece en la query) al valor final del scoreModifier.

sM=1.00+ ^mod+!mod+ *mod+~mod 
	
Se implementaron cuatro operadores de búsqueda: 
	Existe  ^ : Las palabras a la que se aplica a este operador debe aparecer en los documentos que se muestran como resultados de la búsqueda. Si el documento a evaluar no contine la palabra-operando (a las cuales se denominó markers) se disminuye su score en un 100%, en caso contrario permanece inalterado: 
~mod=-1.00
	NoExiste ! : Los marcadores de este operador no deben aparecer en los resultados de búsqueda. Si el documento contiene dicho marcador disminuye su score en 100% en caso contrario permanece inalterado:
!mod=-1.00
	Importancia *: Este operador se puede emplear N veces consecutivas al inicio de la palabra. Por cada vez que se utilice se incrementa el score del documento que contenga los marcadores de este operador en un 35%.
*mod=+N*0.35

	Distancia ~: Este es el único operador binario, se emplea para indicar dos palabras, la distancia entre estas incrementa el score del documento de forma tal que a menor distancia mayor es el incremento. Para utilizarlo escriba primero la primera palabra y luego la segunda comenzando con el operador (Ej.: Harry  ~vacaciones).
 Se tuvo en cuenta que el tamaño de los distintos documentos difiere y esto podría sesgar dicha medida de la distancia, por ello luego de calcular la distancia entre los términos en caracteres, se normaliza la misma dividiéndola entre la cantidad total de caracteres del documento. La función que caracteriza este operador es:
~mod=1.00  -dist(w1,w2)/(doc.Length)

Donde dist(w1,w2) es la menor distancia entre las dos palabras en el documento y doc.Length es la cantidad total de caracteres del documento.
De esta forma, si la distancia entre las palabras es pequeña entonces el segundo término de la expresión tiende a cero y el factor tiende a 1.00 (100%); de lo contrario, si la distancia es grande entonces el segundo término tiende a 1 y el factor tiende a 0%.
# Sugerencias:
Se implementó la funcionalidad Sugerencia basado en la Distancia de Edición o Distancia de Levenshtein. Si alguna palabra de la query no aparece en el vocabulario del corpus y no es una stopword entonces se calcula la distancia de edición entre dicha palabra y todos los términos del vocabulario y sustituimos esta con la palabra del vocabulario con menor distancia de edición.
Luego se sustituye esta palabra en la query.
# Distancia de Levenshtein o distancia de Edición
La Distancia de Levenshtein o distancia de Edición es una medida de que tan similares son dos strings. Se define como el mínimo número de transformaciones (insertar, eliminar o remplazar) que se necesitan para convertir uno de los strings en el otro.

