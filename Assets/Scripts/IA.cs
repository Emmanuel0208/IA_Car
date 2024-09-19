using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IA : MonoBehaviour
{
    public Text scoreText;
    public int capas = 2;
    public int neuronas = 10;
    public Matriz[] pesos;
    public Matriz[] biases;
    Matriz entradas;
    float acceleration;
    float rotation;
    float pitch; // Control de inclinación para aviones
    public float score;
    bool lost = false;

    //ForFitness
    private Vector3 lastPosition;
    private float distanceTraveled = 0;
    float accelerationPR = 0;
    int accelerationProm = 0;

    // Parámetros para mejorar el movimiento hacia adelante
    public float maxRotationAngle = 10f; // Limitar más la rotación
    public float forwardAcceleration = 0.5f; // Aceleración mínima hacia adelante
    public float rotationThreshold = 0.1f; // Rango muerto para giros pequeños

    // Start is called before the first frame update
    void Start()
    {
        Initialize(); // Inicializa todo al principio
    }

    public void Initialize()
    {
        pesos = new Matriz[capas];
        biases = new Matriz[capas];
        entradas = new Matriz(1, 5); // 5 entradas: FD, RD, LD, UD, DD

        for (int i = 0; i < capas; i++)
        {
            if (i == 0)
            {
                pesos[i] = new Matriz(5, neuronas); // 5 entradas en la primera capa
                pesos[i].RandomInitialize();
                biases[i] = new Matriz(1, 5);
                biases[i].RandomInitialize();
            }
            else if (i == capas - 1)
            {
                pesos[i] = new Matriz(3, neuronas); // 3 salidas (rotation, acceleration, pitch)
                pesos[i].RandomInitialize();
                biases[i] = new Matriz(1, 3);
                biases[i].RandomInitialize();
            }
            else
            {
                pesos[i] = new Matriz(neuronas, neuronas); // Capas ocultas
                pesos[i].RandomInitialize();
                biases[i] = new Matriz(1, neuronas);
                biases[i].RandomInitialize();
            }
        }

        // Debug para asegurarnos de que todo se inicializa correctamente
        Debug.Log("Matrices inicializadas correctamente en IA");
    }

    // Update is called once per frame
    void Update()
    {
        if (!lost)
        {
            // Obtén las distancias desde el script Car
            float FD = GetComponent<Car>().ForwardDistance;
            float RD = GetComponent<Car>().RightDistance;
            float LD = GetComponent<Car>().LeftDistance;
            float UD = GetComponent<Car>().UpDistance;
            float DD = GetComponent<Car>().DownDistance;

            // Asigna las entradas a la red neuronal
            entradas.SetAt(0, 0, FD);
            entradas.SetAt(0, 1, RD);
            entradas.SetAt(0, 2, LD);
            entradas.SetAt(0, 3, UD);
            entradas.SetAt(0, 4, DD);

            resolve(); // Resuelve la red neuronal

            // Rango muerto: Ignora giros muy pequeños
            if (Mathf.Abs(rotation) < rotationThreshold)
            {
                rotation = 0;
            }

            // Limitamos la rotación para evitar giros bruscos
            float limitedRotation = Mathf.Clamp(rotation, -maxRotationAngle, maxRotationAngle);

            // Aseguramos que siempre haya una aceleración mínima hacia adelante
            float adjustedAcceleration = Mathf.Max(acceleration, forwardAcceleration);

            // Movimiento en 3D (avión) con rotación limitada y aceleración constante hacia adelante
            transform.Translate(Vector3.forward * adjustedAcceleration);
            transform.eulerAngles = transform.eulerAngles + new Vector3(pitch * 90 * 0.02f, limitedRotation * 90 * 0.02f, 0);

            // Calcular la distancia recorrida
            distanceTraveled += Vector3.Distance(transform.position, lastPosition);
            lastPosition = transform.position;
            accelerationPR += acceleration;
            accelerationProm++;
            SetScore();
        }
    }

    // Resuelve la red neuronal
    void resolve()
    {
        Matriz result;
        result = Activation((entradas * pesos[0]) + biases[0]);
        for (int i = 1; i < capas; i++)
        {
            result = Activation((pesos[i] * result.Transpose()) + biases[i]);
        }
        ActivationLast(result); // Última activación para obtener los valores de salida
    }

    // Función de activación (tangente hiperbólica) para las capas intermedias
    Matriz Activation(Matriz m)
    {
        for (int i = 0; i < m.rows; i++)
        {
            for (int j = 0; j < m.columns; j++)
            {
                m.SetAt(i, j, (float)MathL.HyperbolicTangtent(m.GetAt(i, j)));
            }
        }
        return m;
    }

    // Función de activación para la última capa (obtiene rotation, acceleration, pitch)
    void ActivationLast(Matriz m)
    {
        rotation = (float)MathL.HyperbolicTangtent(m.GetAt(0, 0)); // Rotación limitada
        acceleration = MathL.Sigmoid(m.GetAt(1, 0)); // Mantener la aceleración normal
        pitch = (float)MathL.HyperbolicTangtent(m.GetAt(2, 0)); // Añadir control de pitch (inclinación)
    }

    // Función para calcular el puntaje (fitness)
    void SetScore()
    {
        float FD = GetComponent<Car>().ForwardDistance;
        float RD = GetComponent<Car>().RightDistance;
        float LD = GetComponent<Car>().LeftDistance;
        float s = (FD + RD + LD) / 3;

        // Penalizar giros bruscos o frecuentes
        if (Mathf.Abs(rotation) > rotationThreshold)
        {
            s -= Mathf.Abs(rotation); // Penalización basada en la magnitud de la rotación
        }

        s += ((distanceTraveled * 8) + (acceleration));
        score += (float)Math.Pow(s, 2);
    }

    // Detectar colisión con una pared
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall")
        {
            lost = true;
            Genetics.carsAlive--;
        }
    }
}
