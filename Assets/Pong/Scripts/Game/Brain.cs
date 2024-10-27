using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Brain : MonoBehaviour
{
    [SerializeField] GameObject paddle;
    [SerializeField] GameObject ball;
    Rigidbody2D ballRb;
    float yvel;

    // Edges of level and max speed.
    [SerializeField] float paddleMinY = 8.8f;
    [SerializeField] float paddleMaxY = 17.4f;
    [SerializeField] float paddleMaxSpeed = 15f;

    // Number of times ball touched paddle and back wall.
    float numSaved = 0;
    float numMissed = 0;

    // Neural network
    ANN ann;
    [SerializeField] bool isTestCase = true; // Whether should ANN learn from testcases or just answer them.


    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody2D>();

        // ANN structure is:
        // 6 inputs.
        // 1 output.
        // 1 hidden layer.
        // 4 neurons in hidden layer.
        // Alpha (learning rate): 0.11.
        ann = new(6, 1, 1, 4, 0.11);
    }

    List<double> Run(double bx, double by, double bvx, double bvy, double px, double py, double pv, bool train)
    {
        List<double> inputs = new();
        List<double> outputs = new();

        // Ball x-y location values.
        inputs.Add(bx);
        inputs.Add(by);

        // Ball x-y velocity values.
        inputs.Add(bvx);
        inputs.Add(bvy);

        // Paddle x-y location values.
        inputs.Add(px);
        inputs.Add(py);

        // Desired output.
        outputs.Add(pv);

        // Inputs and outputs can be added as a testcase for training.
        // If already trained, inputs can be used to find an output without training.
        if (train)
        {
            return ann.Train(inputs, outputs);
        }
        else
        {
            return ann.CalcOutput(inputs, outputs);
        }
    }

    void Update()
    {
        // Calculate the y movement of paddle by frame.
        float posy = Mathf.Clamp(paddle.transform.position.y + (yvel * Time.deltaTime * paddleMaxSpeed), paddleMinY, paddleMaxY);

        // Move paddle.
        paddle.transform.position = new Vector3(paddle.transform.position.x, posy, paddle.transform.position.z);

        List<double> output = new();

        // Large number to make it most significant in unity layer system (this is not an ANN layer).
        int layerMask = 1 << 9; // It's 512.

        // Shoot a raycast from ball's position and direction.
        RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, ballRb.velocity, 1000, layerMask);

        if (hit.collider != null)
        {
            // If raycast hits ceiling or floor, calculate and send another raycast to simulate it's reflection.
            if (hit.collider.CompareTag("tops"))
            {
                Vector3 reflection = Vector3.Reflect(ballRb.velocity, hit.normal);
                hit = Physics2D.Raycast(hit.point, reflection, 1000, layerMask);
            }

            // Calculate the desired output for testcase.
            if (hit.collider != null && hit.collider.CompareTag("backwall"))
            {
                float yDelta = hit.point.y - paddle.transform.position.y;

                Vector3 ballPos = ball.transform.position;
                Vector3 paddlePos = paddle.transform.position;

                // Ball position
                // Ball's velocity
                // Paddles position
                // Change in "y" and testcase boolean.
                output = Run(ballPos.x, ballPos.y,
                             ballRb.velocity.x, ballRb.velocity.y,
                             paddlePos.x, paddlePos.y,
                             yDelta, isTestCase);

                yvel = (float)output[0];
            }
        }
        else
        {
            yvel = 0;
        }
    }
}
