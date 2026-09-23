#include <Wire.h>
#include <SPI.h>
#include <Adafruit_Sensor.h>
#include "Adafruit_BME680.h"

#define BME_SCK 13 // BME680
#define BME_MISO 12
#define BME_MOSI 11
#define BME_CS 10

#define DARK_VOLTAGE 0.10  // Werte des Conrad ME709 Datenblatts
#define REF_VOLTAGE  1.30  
#define REF_UVI      6.0

/*#define SCK 13 // SX1276 -> Lora Sender and Reciever
#define MISO 12
#define MOSI 11
#define NSS 10
#define NRESET 9
#define DIO0 2*/

#define UV_SENS A0 // ME709 -> UV Sensor

#define WIND_SPEED_SENS A1 // Anemometer

// BME680 over SPI
Adafruit_BME680 bme(&Wire);//(BME_CS, BME_MOSI, BME_MISO, BME_SCK);

int last_sent_pressure = 0;

void setup() {
  // set Pinmodes
  pinMode(UV_SENS,INPUT);
  pinMode(WIND_SPEED_SENS,INPUT);
  Wire.begin();

  // Setup serial debugging.
  Serial.begin(115200);
  while (!Serial);

  Serial.println("Serial started with baud 9600. Pinmodes assighned.");
  // check if bme is connected.
  if (!bme.begin())
  {
    Serial.println("Unable to find BME680.");
    while(1);
  }
  Serial.print("Found BME680");

  // Oversampling and Gas sensor heating config.
  bme.setTemperatureOversampling(BME680_OS_8X);
  bme.setHumidityOversampling(BME680_OS_2X);
  bme.setPressureOversampling(BME680_OS_4X);
  bme.setIIRFilterSize(BME680_FILTER_SIZE_3);
  bme.setGasHeater(320, 250);
}

void loop() {
  bme.beginReading();
  // The prints before the values are cruitial to parse the message later on.
  Serial.print("Temperature: ");
  Serial.println(bme.temperature);

  Serial.print("Humidity: ");
  Serial.println(bme.humidity);

  Serial.print("IAQ_Index: ");
  Serial.println(log(bme.gas_resistance) + 0.04 * bme.humidity);

  

  float voltage = analogRead(UV_SENS) * (5.0 / 1023.0);

  float uvIndex = (voltage - DARK_VOLTAGE) * REF_UVI / (REF_VOLTAGE - DARK_VOLTAGE);

  if (uvIndex < 0) uvIndex = 0;

  Serial.print("UV_Index: ");
  Serial.println(uvIndex);

  // send pressure only if it has changed by a set margin
  int pressure = bme.pressure;
  if(abs(last_sent_pressure-pressure) > 5)
  {
    // calculate pressure at ground based on height
    int target_height = 382;
    float QNH = 1013.0;
    float Range = 40;
    int least_height_diff = 100;
    float least_diff_press = QNH;
    int height = 341;

    for(float current_test_press = QNH-Range/2; current_test_press < QNH+Range/2; current_test_press += 1)
    {
      int test_alt = bme.readAltitude(current_test_press);
      if (abs(test_alt - height) < least_height_diff)
      {
        least_height_diff = abs(test_alt - height);

        least_diff_press = current_test_press-1;
      }
    }

    Serial.print("QNE_Pressure: ");
    Serial.println(least_diff_press);
    last_sent_pressure = pressure;
  }

  // End of message character
  Serial.println("?");

  bme.endReading();

  // delay for 5 seconds
  delay(5*1000);
}
