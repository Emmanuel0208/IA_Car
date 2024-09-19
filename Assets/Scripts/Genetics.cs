using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Genetics : MonoBehaviour
{
    // Variables públicas para controlar el estado del juego y la genética
    public Text EpochsText;               // Texto para mostrar el número de épocas
    public int epochs = 0;                // Número de épocas actuales
    public GameObject prefab;             // Prefab del coche
    public static int carsAlive;          // Número de coches vivos

    // Parámetros del algoritmo genético
    public int poblacion = 30;            // Tamaño de la población
    public float probDeMutacion = .05f;   // Probabilidad de mutación
    public int mejoresCromosomas = 5;     // Número de mejores cromosomas que se conservarán
    public int peoresCromosomas = 2;      // Número de peores cromosomas que se conservarán
    public int cromosomasParaMutar = 20;  // Número de cromosomas a mutar
    public int mutacionesporCromosoma = 5;// Número de mutaciones por cromosoma

    // Listas para almacenar los coches actuales y la nueva generación
    private List<GameObject> Cars;
    private List<GameObject> newerCars;

    // Método Start: Inicializa la población
    void Start()
    {
        carsAlive = poblacion;            // Inicializar el contador de coches vivos
        Cars = new List<GameObject>();    // Crear la lista de coches actuales
        newerCars = new List<GameObject>(); // Crear la lista de coches para la próxima generación

        // Crear la población inicial de coches
        for (int i = 0; i < poblacion; i++)
        {
            GameObject newObject = Instantiate(prefab) as GameObject;
            Cars.Add(newObject);          // Añadir el coche a la lista de coches
        }
    }

    // Método Update: Controla el ciclo de vida de cada época
    void Update()
    {
        // Actualizar el texto en pantalla con el número de épocas
        EpochsText.text = "Epochs: " + epochs.ToString();

        // Si no quedan coches vivos, pasa a la siguiente época
        if (carsAlive <= 0)
        {
            NextEpoch();      // Crear la nueva generación de coches
            DeleteCars();     // Borrar la población actual
            carsAlive = poblacion;  // Restablecer el número de coches vivos
            epochs++;         // Incrementar el número de épocas
        }
    }

    // Elimina todos los coches de la generación actual
    void DeleteCars()
    {
        for (int i = 0; i < Cars.Count; i++)
        {
            Destroy(Cars[i]);  // Destruir cada coche
        }
        Cars.Clear();          // Vaciar la lista de coches actuales
        Cars = newerCars;      // Asignar la nueva generación de coches a la lista de coches actuales
    }

    // Método NextEpoch: Genera la próxima generación de coches
    void NextEpoch()
    {
        // Ordenar los coches según su puntaje (mejores a peores)
        Cars.Sort((x, y) => x.GetComponent<IA>().score.CompareTo(y.GetComponent<IA>().score));
        List<GameObject> CarsNew = new List<GameObject>();

        // Copiar los mejores coches a la nueva generación
        for (int i = 0; i < mejoresCromosomas; i++)
        {
            CarsNew.Add(Copy(Cars[poblacion - 1 - i]));  // Copiar los mejores
        }

        // Copiar algunos de los peores coches
        for (int i = 0; i < peoresCromosomas; i++)
        {
            CarsNew.Add(Copy(Cars[i]));  // Copiar los peores
        }

        int k = mejoresCromosomas + peoresCromosomas;

        // Generar el resto de la población cruzando coches
        while (k < poblacion)
        {
            int n1 = UnityEngine.Random.Range(0, k - 1);  // Seleccionar coche aleatorio
            int n2 = UnityEngine.Random.Range(0, k - 1);  // Seleccionar otro coche aleatorio
            CarsNew.Add(Cross(CarsNew[n1], CarsNew[n2])); // Cruzar ambos coches
            k++;
        }

        // Aplicar mutaciones a algunos coches
        for (int i = 0; i < cromosomasParaMutar; i++)
        {
            int n1 = UnityEngine.Random.Range(0, poblacion - 1);
            IA iaN = CarsNew[n1].GetComponent<IA>();

            // Verificar antes de mutar biases
            if (iaN.biases != null)
            {
                for (int j = 0; j < iaN.biases.Length; j++)
                {
                    if (iaN.biases[j] != null)
                    {
                        CarsNew[n1].GetComponent<IA>().biases[j].Mutate(mutacionesporCromosoma);
                    }
                }
            }

            // Verificar antes de mutar pesos
            if (iaN.pesos != null)
            {
                for (int j = 0; j < iaN.pesos.Length; j++)
                {
                    if (iaN.pesos[j] != null)
                    {
                        CarsNew[n1].GetComponent<IA>().pesos[j].Mutate(mutacionesporCromosoma);
                    }
                }
            }
        }

        newerCars = CarsNew;  // Actualizar la lista de la nueva generación
    }

    // Función Cross: Cruza dos coches y genera un nuevo coche
    GameObject Cross(GameObject g1, GameObject g2)
    {
        GameObject newObject = Instantiate(prefab) as GameObject;
        IA ia1 = g1.GetComponent<IA>();
        IA ia2 = g2.GetComponent<IA>();

        // Inicializar el nuevo coche
        GameObject r = newObject;
        r.GetComponent<IA>().Initialize();

        // Cruzar los biases entre ambos coches
        for (int i = 0; i < ia1.biases.Length; i++)
        {
            if (ia1.biases[i] != null && ia2.biases[i] != null)
            {
                r.GetComponent<IA>().biases[i] = Matriz.SinglePointCross(ia1.biases[i], ia2.biases[i]);
            }
        }

        // Cruzar los pesos entre ambos coches
        for (int i = 0; i < ia1.pesos.Length; i++)
        {
            if (ia1.pesos[i] != null && ia2.pesos[i] != null)
            {
                r.GetComponent<IA>().pesos[i] = Matriz.SinglePointCross(ia1.pesos[i], ia2.pesos[i]);
            }
        }

        return r;  // Retornar el nuevo coche
    }

    // Función Copy: Copia un coche existente y crea uno nuevo idéntico
    GameObject Copy(GameObject c)
    {
        GameObject newObject = Instantiate(prefab) as GameObject;
        GameObject r = newObject;
        IA ia1 = c.GetComponent<IA>();
        r.GetComponent<IA>().Initialize(); // Inicializar matrices

        // Verificaciones para depuración
        if (ia1 == null) { Debug.LogError("Error: ia1 es null"); }
        if (ia1.biases == null) { Debug.LogError("Error: ia1.biases es null"); }
        if (ia1.pesos == null) { Debug.LogError("Error: ia1.pesos es null"); }

        // Copiar los biases del coche original
        if (ia1.biases != null)
        {
            for (int i = 0; i < ia1.biases.Length; i++)
            {
                if (ia1.biases[i] != null)
                {
                    r.GetComponent<IA>().biases[i] = ia1.biases[i];
                }
                else
                {
                    Debug.LogError("Error: ia1.biases[" + i + "] es null");
                }
            }
        }

        // Copiar los pesos del coche original
        if (ia1.pesos != null)
        {
            for (int i = 0; i < ia1.pesos.Length; i++)
            {
                if (ia1.pesos[i] != null)
                {
                    r.GetComponent<IA>().pesos[i] = ia1.pesos[i];
                }
                else
                {
                    Debug.LogError("Error: ia1.pesos[" + i + "] es null");
                }
            }
        }

        return r;  // Retornar el coche copiado
    }
}
