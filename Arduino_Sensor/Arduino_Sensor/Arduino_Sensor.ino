#include <Wire.h>
#include <SPI.h>
#include <Adafruit_Sensor.h>
#include "Adafruit_BME680.h"

#define SCL A5 // SEN-BME680 -> Humidity, Temperature, Pressure
#define SDA A4 // SEN-BME680

#define SCK 13 // SX1276 -> Lora Sender and Reciever
#define MISO 12
#define MOSI 11
#define NSS 10
#define NRESET 9
#define DIO0 2

#define UV_SENS A0 // ME709 -> UV Sensor

#define WIND_SPEED_SENS A1 // Anemometer

Adafruit_BME680 bme(&Wire);

// Test 

void setup() {
  pinMode(UV_SENS,INPUT);
  pinMode(WIND_SPEED_SENS,INPUT);

  // Setup serial debugging.
  Serial.begin(115200);
  while (!Serial);

  Serial.println("Serial started with baud 9600. Pinmodes assighned.");

  // check if bme is connected.
  if (!bme.beginReading())
  {
    Serial.println("Unable to find BME680.");
    while(1);
  }

  // Oversampling and Gas sensor heating config.
  bme.setTemperatureOversampling(BME680_OS_8X);
  bme.setHumidityOversampling(BME680_OS_2X);
  bme.setPressureOversampling(BME680_OS_4X);
  bme.setIIRFilterSize(BME680_FILTER_SIZE_3);
  bme.setGasHeater(320, 150);
}

void loop() {
  // Get Values

  //bme.temperature
  //bme.pressure
  //bme.humidity
  //bme.gas_resistance
  //bme.readAltitude(float seaLevel)
}
