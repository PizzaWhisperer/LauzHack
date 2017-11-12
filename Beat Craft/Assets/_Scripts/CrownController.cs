using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;


using WebSocketSharp;
using Newtonsoft.Json;
using System.Diagnostics;

using System.Runtime.InteropServices;

public class CrownController : MonoBehaviour {


    public class ToolOption
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class ToolUpdateRootObject
    {
        public string message_type { get; set; }
        public string session_id { get; set; }
        public string show_overlay { get; set; }
        public string tool_id { get; set; }
        public List<ToolOption> tool_options { get; set; }
        public string play_task { get; set; }
    }

    public class CrownRegisterRootObject
    {
        public string message_type { get; set; }
        public string plugin_guid { get; set; }
        public string session_id { get; set; }
        public int PID { get; set; }
        public string execName { get; set; }
    }

    public class TaskOptions
    {
        public string current_tool { get; set; }
        public string current_tool_option { get; set; }
    }

    public class CrownRootObject
    {
        public string message_type { get; set; }
        public int device_id { get; set; }
        public int unit_id { get; set; }
        public int feature_id { get; set; }
        public string task_id { get; set; }
        public string session_id { get; set; }
        public int touch_state { get; set; }
        public TaskOptions task_options { get; set; }
        public int delta { get; set; }
        public int ratchet_delta { get; set; }
        public int time_stamp { get; set; }
        public string state { get; set; }
    }

    public class ToolChangeObject
    {
        public string message_type { get; set; }
        public string session_id { get; set; }
        public string tool_id { get; set; }
    }

    class MyWebSocket
    {
        public static string sessionId = "";
        public static string lastcontext = "";
        public static bool sendContextChange = false;

        [DllImport("kernel32.dll")]
        public static extern bool ProcessIdToSessionId(uint dwProcessID, int pSessionID);

        [DllImport("Kernel32.dll", EntryPoint = "WTSGetActiveConsoleSessionId")]
        public static extern int WTSGetActiveConsoleSessionId();

        private static WebSocket client;
        public static string host1 = "wss://echo.websocket.org";
        public static string host = "ws://localhost:10134";
        public static List<CrownRootObject> crownObjectList = new List<CrownRootObject>();


        public static void toolChange(string contextName)
        {
            try
            {
                ToolChangeObject toolChangeObject = new ToolChangeObject();
                toolChangeObject.message_type = "tool_change";
                toolChangeObject.session_id = sessionId;
                toolChangeObject.tool_id = contextName;

                string s = JsonConvert.SerializeObject(toolChangeObject);
                client.Send(s);

            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }



        public static void updateUIWithDeserializedData(CrownRootObject crownRootObject)
        {
            //CrownRootObject crownRootObject = JsonConvert.DeserializeObject<CrownRootObject>(msg);
            UnityEngine.Debug.Log("Message received 2 : " + crownRootObject.message_type + "\n");
            if (crownRootObject.message_type == "deactivate_plugin")
                return;

            try
            {
                if (crownRootObject.message_type == "crown_turn_event")
                {

                    // received a crown turn event from Craft crown
                    multiplicator += 1.0 * crownRootObject.delta / 180;
                    UnityEngine.Debug.Log("Multiplicator is now at : " + multiplicator + "\n");

                }

                if (crownRootObject.message_type == "crown_press_event")
                {

                    // received a crown turn event from Craft crown
                    if (recording)
                    {
                        recording = false;
                    }
                    else
                    {
                        recording = true;
                    }

                }

            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }

        }

        public static void SetupUIRefreshTimer()
        {

            System.Timers.Timer timer = new System.Timers.Timer(70);
            timer.Enabled = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();

            // reconnection watch dog 
            System.Timers.Timer reconnection_timer = new System.Timers.Timer(30000);
            reconnection_timer.Enabled = true;
            reconnection_timer.Elapsed += new System.Timers.ElapsedEventHandler(connection_watchdog_timer);

            // start watch dog by enabling timer here
            //timer3.Start();



        }

        public static void connection_watchdog_timer(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!client.IsAlive)
            {
                client = null;
                connectWithManager();

            }

        }


        public static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {

                int totalDeltaValue = 0;
                int totalRatchetDeltaValue = 0;
                if (crownObjectList == null || crownObjectList.Count == 0)
                {
                    //Trace.Write("Queue is empty\n");
                    return;
                }
                else
                {
                    //Trace.Write("Queue size is: " + crownObjectList.Count + "\n");
                }

                string currentToolOption = crownObjectList[0].task_options.current_tool_option;

                //Trace.Write("currentToolOption is: " + currentToolOption + "\n");
                CrownRootObject crownRootObject = crownObjectList[0];
                int count = 0;
                for (int i = 0; i < crownObjectList.Count; i++)
                {
                    if (currentToolOption == crownObjectList[i].task_options.current_tool_option)
                    {
                        totalDeltaValue = totalDeltaValue + crownObjectList[i].delta;
                        totalRatchetDeltaValue = totalRatchetDeltaValue + crownObjectList[i].ratchet_delta;
                    }
                    else
                        break;

                    count++;
                }

                if (crownObjectList.Count >= 1)
                {
                    crownObjectList.Clear();

                    crownRootObject.delta = totalDeltaValue;
                    crownRootObject.ratchet_delta = totalRatchetDeltaValue;
                    //Trace.Write("Ratchet delta is :" + totalRatchetDeltaValue + "\n");
                    updateUIWithDeserializedData(crownRootObject);

                }

            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }


        }

        public static void wrapperUpdateUI(string msg)
        {
            UnityEngine.Debug.Log("msg :" + msg + "\n");
            CrownRootObject crownRootObject = JsonConvert.DeserializeObject<CrownRootObject>(msg);
            UnityEngine.Debug.Log("Message received : " + crownRootObject.message_type + "\n");
            if ((crownRootObject.message_type == "crown_turn_event"))
            {
                crownObjectList.Add(crownRootObject);
                UnityEngine.Debug.Log("msg :" + msg + "\n");
            }
            else if(crownRootObject.message_type == "crown_press_event")
            {
                crownObjectList.Add(crownRootObject);
                UnityEngine.Debug.Log("msg :" + msg + "\n");
            }
            else if (crownRootObject.message_type == "register_ack")
            {
                // save the session id as this is used for any communication with Logi Options 
                sessionId = crownRootObject.session_id;
                //toolChange("nothing");
                lastcontext = "";

                if (sendContextChange)
                {
                    sendContextChange = false;
                    MyWebSocket.toolChange("nothing");
                }
                else
                {

                    toolChange("nothing");
                }

            }
            else if (crownRootObject.message_type == "deactivate_plugin" || crownRootObject.message_type == "activate_plugin")
            {
                // our app has been activated or deactivated
            }
            else if (crownRootObject.message_type == "crown_touch_event")
            {
                // crown touch event
                UnityEngine.Debug.Log("crown touch event :" + msg + "\n");
            }


        }
        public static void openUI(string msg)
        {
            string str = msg;
        }

        public static void closeConnection()
        {

        }


        public static void displayError(string msg)
        {
            string str = msg;
        }

        public static void connectWithManager()
        {
            try
            {
                client = new WebSocket(host);

                client.OnOpen += (ss, ee) =>
                    openUI(string.Format("Connected to {0} successfully", host));
                client.OnError += (ss, ee) =>
                    displayError("Error: " + ee.Message);

                client.OnMessage += (ss, ee) =>
                    wrapperUpdateUI(ee.Data);

                client.OnClose += (ss, ee) =>
                    closeConnection();

                client.Connect();

                // build the connection request packet 
                Process currentProcess = Process.GetCurrentProcess();
                CrownRegisterRootObject registerRootObject = new CrownRegisterRootObject();
                registerRootObject.message_type = "register";
                registerRootObject.plugin_guid = "aa7c0911-fbf7-4e87-9c23-25987358303b";
                registerRootObject.execName = "Unity.exe";
                registerRootObject.PID = Convert.ToInt32(currentProcess.Id);
                string s = JsonConvert.SerializeObject(registerRootObject);


                // only connect to active session process
                registerRootObject.PID = Convert.ToInt32(currentProcess.Id);
                int activeConsoleSessionId = WTSGetActiveConsoleSessionId();
                // This line throws errors on unity, replaced by something I believe does the same thing.
                //int currentProcessSessionId = Process.GetCurrentProcess().SessionId;

                client.Send(s);
                // if we are running in active session?
               /*if (currentProcessSessionId == activeConsoleSessionId)
                {
                    client.Send(s);
                }
                else
                {
                    UnityEngine.Debug.Log("Inactive user session. Skipping connect");
                }*/


            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }

        public static void init()
        {
            UnityEngine.Debug.Log("initialize");
            try
            {
                // setup timers 
                SetupUIRefreshTimer();

                // setup connnection 
                connectWithManager();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("Exception " + ex.Message);
                string str = ex.Message;
            }
        }


    }

    private static double multiplicator;
    private static bool recording;

    // Use this for initialization
    void Start () {
        UnityEngine.Debug.Log("Let's go");
        multiplicator = 1;
        recording = false;
        MyWebSocket.init();

	}
	
	// Update is called once per frame
	void Update () {
        UnityEngine.Debug.Log("Velocity Multiplicator : " + multiplicator);
        UnityEngine.Debug.Log("Recording state is now : " + (recording ? "ON" : "OFF"));
    }

    private void OnApplicationFocus(bool focus)
    {
        UnityEngine.Debug.Log("focus");
    }

    private void OnApplicationPause(bool pause)
    {
        UnityEngine.Debug.Log("Paused");
    }
}
