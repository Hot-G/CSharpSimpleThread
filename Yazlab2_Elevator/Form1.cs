using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yazlab2_Elevator
{
    public partial class Form1 : Form
    {
        const int FLOOR_NUMBER = 5, ELEVATOR_NUMBER = 5, FLOOR_TRANSITION_TIME = 200,
            LOGIN_THREAD_TIME = 500, EXIT_THREAD_TIME = 1000, MAIN_FLOOR = 0, CONTROL_TIME = 100;

        List<Customer>[] floors = new List<Customer>[FLOOR_NUMBER];
        Elevator[] elevators = new Elevator[5];
        PictureBox[] elevatorImages;
        Label[] elevatorInfos;
        int UsingElevatorNumber = 1;
        int WillUseElevatorNumber = 1;

        Random random = new Random();
        object _lock = 0;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            for(int i = 0;i < FLOOR_NUMBER; i++)
            {
                floors[i] = new List<Customer>();
            }

            for(int i = 0;i < 5; i++)
            {
                elevators[i] = new Elevator();
            }

            elevatorImages = new PictureBox[5];
            elevatorImages[0] = elevator1Image;
            elevatorImages[1] = elevator2Image;
            elevatorImages[2] = elevator3Image;
            elevatorImages[3] = elevator4Image;
            elevatorImages[4] = elevator5Image;

            elevatorInfos = new Label[5];
            elevatorInfos[0] = elevator1InfoLbl;
            elevatorInfos[1] = elevator2InfoLbl;
            elevatorInfos[2] = elevator3InfoLbl;
            elevatorInfos[3] = elevator4InfoLbl;
            elevatorInfos[4] = elevator5InfoLbl;


            Thread t1 = new Thread(new ThreadStart(LoginThread));
            Thread t2 = new Thread(new ThreadStart(ExitThread));
            Thread t3 = new Thread(new ThreadStart(ElevatorThread));
            Thread t4 = new Thread(new ThreadStart(ControlThread));
            t1.IsBackground = true;
            t2.IsBackground = true;
            t3.IsBackground = true;
            t4.IsBackground = true;
            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
        }


        private void LoginThread()
        {
            while (true)
            {
                lock (_lock)
                {
                    for(int i = 0, n = random.Next(1, 10);i < n; i++)
                    {
                        floors[0].Add(new Customer(random.Next(1, 5)));
                    }
                    Floor0Lbl.Text = "Zemin Kat : " +  floors[0].Count.ToString() + " Kişi";
                }
                
                Thread.Sleep(LOGIN_THREAD_TIME);
            }

        }


        private void ExitThread()
        {
            while (true)
            {
                lock (_lock)
                {
                    for (int i = Clamp(random.Next(1, 5), 0, CanExitCustomer()); i > 0;)
                    {
                        int n = random.Next(1, 4);
                        if (floors[n].Count > 0)
                        {
                            floors[n][0].Target_Floor = 0;
                            floors[n].OrderByDescending(a => a.Target_Floor);
                            i--;
                        }
                    }
                }
                Thread.Sleep(EXIT_THREAD_TIME);
            }
        }

        private void ElevatorThread()
        {
            while (true)
            {
                lock (_lock)
                {
                    for (int i = 0; i < UsingElevatorNumber; i++)
                     {
                        //ASANSÖRÜN KONUMUNU AYARLA
                        elevatorImages[i].Location = new Point(150 * (i + 1) + 50, (450 - elevators[i].CurrentFloor * 100) - 28);
                        //MÜŞTERİLERİ BİNDİRİP İNDİR
                        if (elevators[i].CurrentFloor == MAIN_FLOOR)
                        {
                            for (int j = 0, n = elevators[i].GetCustomerNumber(); j < n; j++)
                            {
                                if (elevators[i].customers[j].Target_Floor == 0)
                                {
                                    elevators[i].customers.RemoveAt(j);
                                    j--;
                                    n--;
                                }
                            }

                            if(WillUseElevatorNumber < UsingElevatorNumber)
                            {
                                UsingElevatorNumber = WillUseElevatorNumber;
                            }
                            else
                            {
                                for (int j = 0, n = Clamp(floors[0].Count, 0, elevators[i].GetEmptySlot()); j < n; j++)
                                {
                                    elevators[i].customers.Add(floors[0][0]);
                                    floors[0].RemoveAt(0);
                                }
                            }
                                                       
                        }
                        else
                        {
                            // O KATTAKİ MÜŞTERİLERİ İNDİR
                            for (int j = 0, n = elevators[i].GetCustomerNumber(); j < n; j++)
                            {
                                if (elevators[i].customers[j].Target_Floor == elevators[i].CurrentFloor)
                                {
                                    floors[elevators[i].CurrentFloor].Add(elevators[i].customers[j]);
                                    elevators[i].customers.RemoveAt(j);
                                    j--;
                                    n--;
                                }
                            }
                            // O KATTAKİ MÜŞTERİLERİ AL
                            for (int j = 0, n = floors[elevators[i].CurrentFloor].Count; j < n; j++)
                            {
                                if (elevators[i].GetEmptySlot() == 0)
                                    break;

                                if (floors[elevators[i].CurrentFloor][j].Target_Floor == 0)
                                {
                                    elevators[i].customers.Add(floors[elevators[i].CurrentFloor][j]);
                                    floors[elevators[i].CurrentFloor].RemoveAt(j);
                                    j--;
                                    n--;
                                }
                            }
                        }
                        switch (elevators[i].CurrentFloor)
                        {
                            case 0:
                                Floor0Lbl.Text = "Zemin Kat : " + floors[0].Count.ToString() + " Kişi";
                                break;
                            case 1:
                                Floor1Lbl.Text = "1. Kat : " + floors[1].Count.ToString() + " Kişi";
                                break;
                            case 2:
                                Floor2Lbl.Text = "2. Kat : " + floors[2].Count.ToString() + " Kişi";
                                break;
                            case 3:
                                Floor3Lbl.Text = "3. Kat : " + floors[3].Count.ToString() + " Kişi";
                                break;
                            case 4:
                                Floor4Lbl.Text = "4. Kat : " + floors[4].Count.ToString() + " Kişi";
                                break;
                        }

                        elevatorInfos[i].Text = (i + 1).ToString() + ". Asansör " + elevators[i].GetCustomerNumber().ToString() + " Kişi";

                        elevators[i].CurrentFloor += elevators[i].Direction;

                        if (elevators[i].CurrentFloor == FLOOR_NUMBER - 1 || elevators[i].CurrentFloor == MAIN_FLOOR)
                        {
                            elevators[i].Direction *= -1;
                        }
                    }

                    

                }

                Thread.Sleep(FLOOR_TRANSITION_TIME);
            }
        }

        private void ControlThread()
        {
            while (true)
            {
                lock (_lock)
                {
                    if (GetNumberInQuery() > UsingElevatorNumber * 20)
                    {
                        UsingElevatorNumber = Clamp(UsingElevatorNumber + 1, 1, 5);
                        WillUseElevatorNumber = UsingElevatorNumber;
                    }

                    if(GetNumberInQuery() < (UsingElevatorNumber - 1) * 20 && elevators[UsingElevatorNumber - 1].CurrentFloor == MAIN_FLOOR)
                    {
                        WillUseElevatorNumber = Clamp(UsingElevatorNumber - 1, 1, 5);
                    }
                }

                Thread.Sleep(CONTROL_TIME);
            }
        }

        private int Clamp(int value, int minValue, int maxValue)
        {
            if (value < minValue)
                return minValue;

            if (value > maxValue)
                return maxValue;

            return value;
        }

        private int CanExitCustomer()
        {
            int toplam = 0;

            for(int i = 1;i < 5; i++)
            {
                for(int j = 0, n = floors[i].Count;j < n; j++)
                {
                    if(floors[i][j].Target_Floor != 0)
                    {
                        toplam++;
                    }
                }
            }

            return toplam;

        }

        private int GetNumberInQuery()
        {
            int toplam = 0;

            for(int i = 0;i < FLOOR_NUMBER; i++)
            {
                if(i == MAIN_FLOOR)
                {
                    toplam += floors[i].Count;
                }
                else
                {
                    for(int j = 0;j < floors[i].Count; j++)
                    {
                        if(floors[i][j].Target_Floor == 0)
                        {
                            toplam++;
                        }
                    }
                }

            }

            return toplam;       
        }    

    }
}
