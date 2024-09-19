using UnityEngine;

public class Car : MonoBehaviour
{
    // Declaración de variables para la posición y las direcciones de los rayos
    Vector3 pos;
    Vector3 forward;  // Rayo hacia adelante
    Vector3 left;     // Rayo hacia la izquierda
    Vector3 right;    // Rayo hacia la derecha
    Vector3 up;       // Rayo inclinado 45 grados hacia adelante y arriba
    Vector3 down;     // Rayo inclinado 45 grados hacia adelante y abajo

    // Variables públicas que almacenan las distancias detectadas por los rayos
    public float ForwardDistance = 0;
    public float LeftDistance = 0;
    public float RightDistance = 0;
    public float UpDistance = 0;
    public float DownDistance = 0;

    // Update se llama una vez por frame para actualizar la lógica de raycasting
    private void Update()
    {
        // Definir las direcciones de los rayos hacia adelante, izquierda y derecha multiplicados por la distancia (30 unidades)
        forward = transform.TransformDirection(Vector3.forward) * 30;
        left = transform.TransformDirection(new Vector3(.5f, 0, 1)) * 30;
        right = transform.TransformDirection(new Vector3(-.5f, 0, 1)) * 30;

        // Definir las direcciones de los rayos inclinados hacia adelante y arriba/abajo (45 grados)
        up = transform.TransformDirection(new Vector3(0, 0.5f, 1)) * 30;  // 45 grados adelante y arriba
        down = transform.TransformDirection(new Vector3(0, -0.5f, 1)) * 30;  // 45 grados adelante y abajo

        // Establecer la posición de origen de los rayos, 2 unidades sobre la posición actual del objeto
        pos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);

        // Dibujar rayos en la escena para depuración visual
        Debug.DrawRay(pos, forward, Color.red);  // Rayo hacia adelante
        Debug.DrawRay(pos, left, Color.red);     // Rayo hacia la izquierda
        Debug.DrawRay(pos, right, Color.red);    // Rayo hacia la derecha
        Debug.DrawRay(pos, up, Color.red);       // Rayo inclinado hacia adelante y arriba
        Debug.DrawRay(pos, down, Color.red);     // Rayo inclinado hacia adelante y abajo
    }

    // FixedUpdate se llama en intervalos de tiempo fijos y se usa para la física (raycasting en este caso)
    private void FixedUpdate()
    {
        RaycastHit hit;   // Variable que almacena la información del impacto del rayo hacia adelante
        RaycastHit hit2;  // Impacto del rayo hacia la izquierda
        RaycastHit hit3;  // Impacto del rayo hacia la derecha
        RaycastHit hit4;  // Impacto del rayo inclinado hacia adelante y arriba
        RaycastHit hit5;  // Impacto del rayo inclinado hacia adelante y abajo

        // Definir los rayos que parten de la posición 'pos' en las direcciones correspondientes
        Ray RF = new Ray(pos, forward);  // Rayo hacia adelante
        Ray RL = new Ray(pos, left);     // Rayo hacia la izquierda
        Ray RR = new Ray(pos, right);    // Rayo hacia la derecha
        Ray RU = new Ray(pos, up);       // Rayo inclinado hacia adelante y arriba
        Ray RD = new Ray(pos, down);     // Rayo inclinado hacia adelante y abajo

        // Inicializar las distancias a 1 (máxima distancia detectada)
        ForwardDistance = 1;
        LeftDistance = 1;
        RightDistance = 1;
        UpDistance = 1;
        DownDistance = 1;

        // Realizar el raycasting y actualizar las distancias si se detectan objetos a menos de 30 unidades
        if (Physics.Raycast(pos, forward, out hit, 30))
        {
            ForwardDistance = hit.distance / 30;  // Normalizar la distancia para obtener un valor entre 0 y 1
        }
        if (Physics.Raycast(pos, left, out hit2, 30))
        {
            LeftDistance = hit2.distance / 30;    // Normalizar la distancia para la izquierda
        }
        if (Physics.Raycast(pos, right, out hit3, 30))
        {
            RightDistance = hit3.distance / 30;   // Normalizar la distancia para la derecha
        }
        if (Physics.Raycast(pos, up, out hit4, 30))
        {
            UpDistance = hit4.distance / 30;      // Normalizar la distancia para el rayo inclinado hacia arriba
        }
        if (Physics.Raycast(pos, down, out hit5, 30))
        {
            DownDistance = hit5.distance / 30;    // Normalizar la distancia para el rayo inclinado hacia abajo
        }

        
    }
}
