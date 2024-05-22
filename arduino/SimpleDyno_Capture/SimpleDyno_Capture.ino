/*
  Sketch for use with SimpleDyno
  Developed on Arduino Uno Platform
  DamoRC - 2013-2014
  
  ALWAYS use the Sketch distributed with each new version of SimpleDyno
  
  Timer capture on pin 8 added by Janne Ahonen 2023
  
  Transmits:
    1 x Session timestamp 
    1 x Interrupt timestamp and 1 x time interval since last capture ICP1 / Pin8 / RPM1
    1 x Interrupt timestamp and 1 x time interval since last interrupt for INT1 / Pin3 / RPM2
    6 x Analog Inputs (A0 and A1 are Voltage and Current, A2 and A3 are Temperature, A4 and A5 are open)

  Values are comma delimited
  Baud rates selected in SD must match coded values in this Sketch.
*/

  volatile uint32_t ts1 = 0;
  volatile uint8_t ts1_ext = 0;
  volatile uint32_t elapsed1 = 0;
  volatile uint8_t elapsed1_ext = 0; 

  volatile unsigned long time2 = 0;
  volatile unsigned long Oldtime2 = 0;
  volatile unsigned long TempTime2 = 0;
  String AllResult = "";

  volatile uint32_t u32TmrHi = 0; 

  const String timeDecimals[16] = 
  {
    "", 
    ".0625", 
    ".125", 
    ".1875", 
    ".25", 
    ".3125", 
    ".375",
    ".4375",
    ".5",
    ".5625",
    ".625",
    ".6875",
    ".75",
    ".8125",
    ".875", 
    ".9375"
  };

#define RISING_EDGE _BV(ICES1)
#define FALLING_EDGE 0

void setup() {
  // Initialize serial communication
  // Ensure that Baud rate specified here matches that selected in SimpleDyno
  // Availailable Baud rates are:
  // 9600, 14400, 19200, 28800, 38400, 57600, 115200
  Serial.begin(115200);

  // Initialize interupts (Pin3 in interrupt 1 = RPM2)
  attachInterrupt(1,channel2,FALLING);

  // Initialize capture

  noInterrupts();  // protected code
  // reset Timer 1
  TCCR1A = 0;
  TCCR1B = 0;
  TCNT1 = 0;
  TIMSK1 = 0;

  TIFR1 |= _BV(ICF1); // clear Input Capture Flag so we don't get a bogus interrupt
  TIFR1 |= _BV(TOV1); // clear Overflow Flag so we don't get a bogus interrupt

  TCCR1B = _BV(CS10) | FALLING_EDGE; // start Timer 1, no prescaler, Input Capture Edge Select (0=Falling, 1=Rising)

  TIMSK1 |= _BV(ICIE1); // Enable Timer 1 Input Capture Interrupt
  TIMSK1 |= _BV(TOIE1); // Enable Timer 1 Overflow Interrupt
  interrupts();
}

//
// Send data only after 20 ms has elapsed after the last send. 
// This improves the calculated torque accuracy (keeps derivation 
// time constant and makes it independent of the used baud rate).
//
// This can be tweaked for each case to get optimum update rate vs resolution.
//

#define SEND_TIME_INTERVAL (20000)

void loop() {
  uint32_t u32tsCH1;
  uint8_t u8tsCH1ext;
  uint32_t u32elapsedCH1;
  uint8_t u8elapsedCH1ext;
  uint32_t u32tsCH2;
  uint32_t u32elapsedCH2;
  uint32_t u32ts;
  uint16_t u16ADCResult;
  static uint32_t u32tsLast = 0;

  u32ts = micros();
  
  if ((u32ts - u32tsLast) > SEND_TIME_INTERVAL) 
  {
    noInterrupts();
    u32tsCH1 = ts1;
    u8tsCH1ext = ts1_ext;
    u32elapsedCH1 = elapsed1;
    u8elapsedCH1ext = elapsed1_ext;
    u32tsCH2 = TempTime2;
    u32elapsedCH2 = time2;
    interrupts();

    AllResult = "";
    AllResult += u32ts;
    AllResult += ",";
    AllResult += u32tsCH1 + timeDecimals[u8tsCH1ext];
    AllResult += ",";
    AllResult += u32elapsedCH1 + timeDecimals[u8elapsedCH1ext];
    AllResult += ",";
    AllResult += u32tsCH2;
    AllResult += ",";
    AllResult += u32elapsedCH2;
    for (int i = 0; i < 6;i++)
    {
      u16ADCResult = analogRead(i);
      AllResult += ",";
      AllResult += u16ADCResult;
    }
    Serial.println (AllResult);
    Serial.flush();
    u32tsLast = u32ts;
  }
}

ISR(TIMER1_OVF_vect)
{
  u32TmrHi++;
}

ISR(TIMER1_CAPT_vect)
{
  uint32_t tsHigh = u32TmrHi;
  uint32_t tsNow;
  static uint32_t tsOld = 0;

  // If an overflow happened but has not been handled yet
  // and the timer count was close to zero, count the
  // overflow as part of this time.

  if ((TIFR1 & _BV(TOV1)) && (ICR1 < 1024))
  {
    tsHigh++;
  }

  tsNow = (tsHigh << 16) | ICR1;

  // Get time stamp of the event so that ts1 unit is microseconds
  // and extra 4 bits goes into ts1_ext as extended resolution

  ts1 = ((tsHigh & 0xF0000) << 12) + (tsNow >> 4);
  ts1_ext = tsNow & 0xF;

  // Calculate elapsed time so that time1 unit is microseconds 
  // and extra 4 bits goes into time1_ext as extended resolution

  elapsed1 = tsNow-tsOld;
  elapsed1_ext = elapsed1 & 0xF;
  elapsed1 >>= 4;

  tsOld = tsNow;
}

//Interrupt routine for RPM2
void channel2()
{
  TempTime2 = micros();
  time2 = TempTime2-Oldtime2;
  Oldtime2 = TempTime2;
}
