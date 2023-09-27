#include <Arduino.h>
#include <WiFi.h>
#include <ESPAsyncWebServer.h>
#include <HTTPClient.h>


// Pins
#define RED_LED_PIN 23
#define GREEN_LED_PIN 22
#define BUTTON_PIN 32
#define MOS_PIN 19


// States
int RED_LED_STATE = HIGH;
int GREEN_LED_STATE = LOW;
int BUTTON_STATE = HIGH;
int MOS_STATE = LOW;


// Button debounce
int LAST_BUTTON_STATE = HIGH;
unsigned long LAST_DEBOUNCE_TIME = 0;
unsigned long DEBOUNCE_DELAY = 50;


// Server
const char* ssid = "***";
const char* password = "***";

IPAddress staticip(192, 168, 1, 200);
IPAddress gateway(192, 168, 1, 254);
IPAddress subnet(255, 255, 255, 0);
IPAddress dns(8, 8, 8, 8);

AsyncWebServer server(80);
HTTPClient http;

// Func(s)
void SWITCH() {
  int BTN_DIG_READING = digitalRead(BUTTON_PIN);
  
  if (BTN_DIG_READING != LAST_BUTTON_STATE) {
    LAST_DEBOUNCE_TIME = millis();
  }
  
  if ((millis() - LAST_DEBOUNCE_TIME) > DEBOUNCE_DELAY) {
    if (BTN_DIG_READING != BUTTON_STATE) {
      BUTTON_STATE = BTN_DIG_READING;

      if (BUTTON_STATE == LOW) {
        if (RED_LED_STATE == LOW) {
          RED_LED_STATE = HIGH;
          GREEN_LED_STATE = LOW;
          MOS_STATE = LOW;
        } 
        else {
          RED_LED_STATE = LOW;
          GREEN_LED_STATE = HIGH;
          MOS_STATE = HIGH;
        }
      }
    }
  }
  digitalWrite(RED_LED_PIN, RED_LED_STATE);
  digitalWrite(GREEN_LED_PIN, GREEN_LED_STATE);
  digitalWrite(MOS_PIN, MOS_STATE);

  LAST_BUTTON_STATE = BTN_DIG_READING;
}


// Setup
void setup() {
  // Initialize pins
  Serial.begin(9600);
  Serial.println("Started");

  pinMode(RED_LED_PIN, OUTPUT);
  pinMode(GREEN_LED_PIN, OUTPUT);
  pinMode(MOS_PIN, OUTPUT);

  pinMode(BUTTON_PIN, INPUT_PULLUP);


  // Webserver
  WiFi.config(staticip, gateway, subnet, dns);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Csatlakozás...");
  }

  Serial.println("Csatlakozva!");
  Serial.println(WiFi.localIP().toString());


  // Route to handle requests
  // ON
  server.on("/on", HTTP_GET, [](AsyncWebServerRequest *request){
    String message = "200 ON";
    request->send(200, "text/plain", message);

    GREEN_LED_STATE = HIGH;
    RED_LED_STATE = LOW;
  });

  // OFF
  server.on("/off", HTTP_GET, [](AsyncWebServerRequest *request){
    String message = "200 OFF";
    request->send(200, "text/plain", message);

    GREEN_LED_STATE = LOW;
    RED_LED_STATE = HIGH;
  });

  // SYNC
  server.on("/sync", HTTP_GET, [](AsyncWebServerRequest *request){
    String message = "500 ERR";
    if (GREEN_LED_STATE == HIGH && RED_LED_STATE == LOW) {
      message = "102 ON";
    }
    else if (RED_LED_STATE == HIGH && GREEN_LED_STATE == LOW) {
      message = "102 OFF";
    }

    request->send(200, "text/plain", message);
  });

  // Start server
  server.begin();
}



// Loop
void loop() {
  SWITCH();
  digitalWrite(GREEN_LED_PIN, GREEN_LED_STATE);
  digitalWrite(RED_LED_PIN, RED_LED_STATE);
}