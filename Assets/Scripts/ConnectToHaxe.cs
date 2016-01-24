using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class ConnectToHaxe : MonoBehaviour
{
    public GameObject rock;
    public GameObject asteroid;
    public GameObject planet;
    public GameObject sun;
    
    private Dictionary<int, GameObject> idMap;

    private Socket socket;
    private byte[] bytes;
    private IPEndPoint remoteEP;

    void Stop()
    {

    }

    void Start()
    {
        Application.runInBackground = true;
        Debug.Log("Net listener started!");
        // Data buffer for incoming data.
        bytes = new byte[1024];

        // Connect to a remote device.
        try
        {
            // Establish the remote endpoint for the socket.
            // This example uses port 30303 on the local computer.
            IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            remoteEP = new IPEndPoint(ipAddress, 30303);

            // Create a TCP/IP  socket.
            socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(remoteEP);
            Debug.Log("Socket connected to {0}" +
                socket.RemoteEndPoint.ToString());

            idMap = new Dictionary<int, GameObject>();

            socket.Blocking = false;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    void Update()
    {
        // Connect the socket to the remote endpoint. Catch any errors.
        try
        {
            /*// Encode the data string into a byte array.
            byte[] msg2 = Encoding.ASCII.GetBytes("Connected.");

            // Send the data through the socket.
            int bytesSent = socket.Send(msg2);*/

            // Receive the response from the remote device.
            int bytesRec;

            string fullMsg = "";
            while ((bytesRec = socket.Receive(bytes)) > 0)
            {
                fullMsg += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                if (fullMsg.EndsWith(";"))
                {
                    //We have a full message
                    MatchCollection matches = Regex.Matches(fullMsg, @"([^\{\w\d\s]?)([\w\d]+)(\s+)?(=>)(\s+)?([\w\d+-]+)");
                    var messageRecieved = new Dictionary<String, String>();

                    foreach (Match match in matches)
                    {
                        String leftSideRegex = @"[\w\d]+(?=(\s+(=>)))";
                        String rightSideRegex = @"(?<=(=>(\s+)))[\+\-\d\w]+";

                        messageRecieved[Regex.Match(match.Value, leftSideRegex).Value.ToLower()] = Regex.Match(match.Value, rightSideRegex).Value.ToLower();
                    }

                    switch (messageRecieved["type"])
                    {
                        case "command":
                            switch (messageRecieved["command"])
                            {
                                case "let_there_be":
                                    Debug.Log(messageRecieved.ToString());
                                    string objectToCreate = messageRecieved["object"];
                                    float radius = float.Parse(messageRecieved["radius"]);
                                    float angleXY = float.Parse(messageRecieved["anglexy"]);
                                    float angleYZ = float.Parse(messageRecieved["angleyz"]);
                                    int orbits = int.Parse(messageRecieved["orbits"]);
                                    int idOfObject = int.Parse(messageRecieved["createdid"]);

                                    GameObject newGameObject = createGameObject(objectToCreate, radius, angleXY, angleYZ);
                                    Orbit orbitComponent = newGameObject.AddComponent<Orbit>();
                                    if (orbits != -1)
                                    {
                                        newGameObject.transform.position += idMap[orbits].transform.position;
                                        orbitComponent.orbitCenter = idMap[orbits];
                                    }
                                    else orbitComponent.orbitCenter = Camera.main.gameObject;

                                    orbitComponent.setupAxis(angleXY, angleYZ);
                                    orbitComponent.orbitPeriod = UnityEngine.Random.Range(30, 60);

                                    idMap[idOfObject] = newGameObject;

                                    break;
                            }

                            break;
                    }

                    //Reset stuff for next msg
                    bytes = new byte[1024];
                    fullMsg = "";
                }
            }
        }
        catch (ArgumentNullException ane)
        {
            Debug.Log("ArgumentNullException : {0}" + ane.ToString());
        }
        catch (SocketException se)
        {
            //Debug.Log("SocketException : {0}" + se.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Unexpected exception : {0}" + e.ToString());
        }
    }

    GameObject createGameObject(String name, float radius, float angleXY, float angleYZ)
    {
        switch (name)
        {
            case "star":
                return Instantiate(sun, sphericalToCartesian(radius, angleXY, angleYZ), Quaternion.Euler(0, 0, 0)) as GameObject;
                
            case "planet":
                return Instantiate(planet, sphericalToCartesian(radius, angleXY, angleYZ), Quaternion.Euler(0, 0, 0)) as GameObject;
                
            case "moon":
                return Instantiate(rock, sphericalToCartesian(radius, angleXY, angleYZ), Quaternion.Euler(0, 0, 0)) as GameObject;
                
            case "asteroid":
                return Instantiate(asteroid, sphericalToCartesian(radius, angleXY, angleYZ), Quaternion.Euler(0, 0, 0)) as GameObject;
        }

        throw new Exception("Unrecognized name");
    }

    void closeSocket()
    {
        if (socket == null) return;
        // Release the socket.
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

    void OnApplicationQuit()
    {
        closeSocket();
    }

    Vector3 sphericalToCartesian(float radius, float angleXY, float angleYZ)
    {
        Vector3 result = new Vector3(radius, 0, 0);
        result = Quaternion.AngleAxis(angleXY - 90, Vector3.up) * Quaternion.AngleAxis(angleYZ, Vector3.forward) * result;
        return result;
    }
}