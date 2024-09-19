using System;

public class Matriz
{
    // Matriz representada como un arreglo 2D de flotantes
    float[,] mat;
    public int rows;    // Número de filas de la matriz
    public int columns; // Número de columnas de la matriz

    // Constructor: Inicializa una matriz con un tamaño específico
    public Matriz(int _rows, int _columns)
    {
        rows = _rows;
        columns = _columns;
        mat = new float[rows, columns]; // Crea la matriz con las dimensiones especificadas
    }

    // Obtiene el valor en la posición (x, y) de la matriz
    public float GetAt(int x, int y)
    {
        return mat[x, y];
    }

    // Establece un valor en la posición (x, y) de la matriz
    public void SetAt(int x, int y, float v)
    {
        mat[x, y] = v;
    }

    // Sobrecarga del operador + para sumar dos matrices de igual tamaño
    public static Matriz operator +(Matriz m1, Matriz m2)
    {
        // Solo suma si las matrices tienen las mismas dimensiones
        if (m1.rows == m2.rows && m1.columns == m2.columns)
        {
            for (int i = 0; i < m1.rows; i++)
            {
                for (int j = 0; j < m1.columns; j++)
                {
                    // Suma los elementos correspondientes de las dos matrices
                    m1.SetAt(i, j, m1.GetAt(i, j) + m2.GetAt(i, j));
                }
            }
        }
        return m1; // Retorna la matriz sumada
    }

    // Sobrecarga del operador * para multiplicar dos matrices
    public static Matriz operator *(Matriz m1, Matriz m2)
    {
        Matriz mat2 = new Matriz(0, 0); // Matriz vacía que se retorna si la multiplicación no es posible

        // Solo multiplica si el número de columnas de la primera matriz es igual al número de filas de la segunda
        if (m1.columns == m2.rows)
        {
            Matriz mat3 = new Matriz(m1.rows, m2.columns); // Crear la matriz resultado

            // Multiplicación de matrices
            for (int i = 0; i < m1.rows; i++)
            {
                for (int k = 0; k < m2.columns; k++)
                {
                    for (int j = 0; j < m2.rows; j++)
                    {
                        // Sumar el producto de los elementos correspondientes
                        mat3.SetAt(i, k, mat3.GetAt(i, k) + m1.GetAt(i, j) * m2.GetAt(j, k));
                    }
                }
            }
            return mat3; // Retorna la matriz resultante
        }
        else
        {
            // Error si las dimensiones no son adecuadas para la multiplicación
            //UnityEngine.Debug.LogError("FAIL");
            return mat2; // Retorna una matriz vacía
        }
    }

    // Inicializa la matriz con valores aleatorios entre -100 y 100
    public void RandomInitialize()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                mat[i, j] = UnityEngine.Random.Range(-100f, 100f); // Asignar un valor aleatorio
            }
        }
    }

    // Transpone la matriz (intercambia filas por columnas)
    public Matriz Transpose()
    {
        Matriz m = new Matriz(columns, rows); // Crear la matriz transpuesta
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                m.SetAt(j, i, mat[i, j]); // Asigna el valor transpuesto
            }
        }
        return m; // Retorna la matriz transpuesta
    }

    // Cruce de un solo punto entre dos matrices
    public static Matriz SinglePointCross(Matriz m1, Matriz m2)
    {
        Matriz mr = new Matriz(m1.rows, m1.columns); // Crear la nueva matriz resultante del cruce
        int crosspointC = UnityEngine.Random.Range(0, m1.columns); // Punto de cruce para columnas
        int crosspointR = UnityEngine.Random.Range(0, m1.rows);    // Punto de cruce para filas

        // Solo realiza el cruce si ambas matrices tienen las mismas dimensiones
        if (m1.columns == m2.columns && m1.rows == m2.rows)
        {
            for (int i = 0; i < m1.rows; i++)
            {
                for (int j = 0; j < m1.columns; j++)
                {
                    // Asigna valores de la primera o segunda matriz dependiendo del punto de cruce
                    if (i < crosspointC || j < crosspointR)
                    {
                        mr.SetAt(i, j, m1.GetAt(i, j));
                    }
                    else
                    {
                        mr.SetAt(i, j, m2.GetAt(i, j));
                    }
                }
            }
            return mr; // Retorna la matriz resultante del cruce
        }
        UnityEngine.Debug.LogError("BAD SINGLEPOINTCROSS"); // Error si las dimensiones no coinciden
        return null; // Retorna null si el cruce no es posible
    }

    // Mutación de la matriz en un número específico de posiciones
    public void Mutate(int mut)
    {
        for (int i = 0; i < mut; i++)
        {
            int n1 = UnityEngine.Random.Range(0, rows - 1);   // Selección aleatoria de una fila
            int n2 = UnityEngine.Random.Range(0, columns - 1); // Selección aleatoria de una columna
            mat[n1, n2] = mat[n1, n2] + UnityEngine.Random.Range(-100, 100); // Mutar el valor seleccionado
        }
    }

    // Imprime la matriz en la consola
    public void print()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Console.WriteLine(mat[i, j]); // Imprime cada elemento de la matriz
            }
        }
    }
}
