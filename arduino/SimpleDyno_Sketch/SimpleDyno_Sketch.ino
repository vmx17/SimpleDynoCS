/*
  Sketch for use with SimpleDyno
  Developed on Arduino Uno Platform
  DamoRC - 2013-2014
  Edited by Lassi Niemisto 2021

  ALWAYS use the Sketch distributed with each new version of SimpleDyno

  Transmits:
    1 x Session timestamp
    1 x Interrupt timestamp and 1 x time interval since last interrupt for INT0 / Pin2 / RPM1
    1 x Interrupt timestamp and 1 x time interval since last interrupt for INT1 / Pin3 / RPM2
    6 x Analog Inputs (A0 and A1 are Voltage and Current, A2 and A3 are Temperature, A4 and A5 are open)
  Values are comma delimeted
  Baud rates selected in SD must match coded values in this Sketch.
*/

const int NumPortsToRead = 6;
int AnalogResult[NumPortsToRead];
volatile unsigned long TimeStamp = 0;
volatile unsigned long time1 = 0;
volatile unsigned long time2 = 0;
volatile unsigned long Oldtime1 = 0;
volatile unsigned long Oldtime2 = 0;
volatile unsigned long TempTime1 = 0;
volatile unsigned long TempTime2 = 0;
String AllResult = "";
volatile bool mainReading = false;
volatile bool delayedEventChannel1 = false;
volatile bool delayedEventChannel2 = false;

void setup() {
  // Initialize serial communication
  // Ensure that Baud rate specified here matches that selected in SimpleDyno
  // Availailable Baud rates are:
  // 9600, 14400, 19200, 28800, 38400, 57600, 115200
  Serial.begin(38400);
  // Initialize interupts (Pin2 is interrupt 0 = RPM1, Pin3 in interrupt 1 = RPM2)
  attachInterrupt(0, channel1, RISING);
  attachInterrupt(1, channel2, RISING);
}

void loop() {

  // Read the latest sample data with a protection flag that disallows modifying it during the read
  mainReading = true;
  unsigned long local_TempTime1 = TempTime1;
  unsigned long local_time1 = time1;
  unsigned long local_TempTime2 = TempTime2;
  unsigned long local_time2 = time2;
  mainReading = false;

  // If interrupts arrived during the above read, re-execute those events now 
  if (delayedEventChannel1)
  {
    delayedEventChannel1 = false;
    channel1();
  }
  if (delayedEventChannel2)
  {
    delayedEventChannel2 = false;
    channel2();
  }

  AllResult = "";
  AllResult += micros();
  AllResult += ",";
  AllResult += local_TempTime1;
  AllResult += ",";
  AllResult += local_time1;
  AllResult += ",";
  AllResult += local_TempTime2;
  AllResult += ",";
  AllResult += local_time2;
  for (int Looper = 0; Looper < NumPortsToRead; Looper++) {
    AnalogResult[Looper] = analogRead(Looper);
    AllResult += ",";
    AllResult += AnalogResult[Looper];
  }
  Serial.println (AllResult);
  Serial.flush();
  delay(1);
}

//Interrupt routine for RPM1
void channel1() {
  if (mainReading)
  {
    delayedEventChannel1 = true;
  } else {
    TempTime1 = micros();
    time1 = TempTime1 - Oldtime1;
    Oldtime1 = TempTime1;
  }
}

//Interrupt routine for RPM2
void channel2() {
  if (mainReading)
  {
    delayedEventChannel2 = true;
  } else {
    TempTime2 = micros();
    time2 = TempTime2 - Oldtime2;
    Oldtime2 = TempTime2;
  }
}
