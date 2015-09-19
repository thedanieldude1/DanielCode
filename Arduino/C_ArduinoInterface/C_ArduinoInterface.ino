void setup() {
  // put your setup code here, to run once:
  pinMode(4, OUTPUT);
  Serial.begin(9600);


}

void loop() {
  // put your main code here, to run repeatedly:


    if(Serial.readString()=="ON"){
    digitalWrite(4, HIGH);
    Serial.println("Message Recieved: ON!");
    }
    
    delay(100);
    if(Serial.readString()=="OFF"){
      digitalWrite(4, LOW);
    }    
  delay(100);
   


}
