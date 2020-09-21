package main

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"time"
)

type incomingEvent struct {
	Data interface{} `json:"data"`
}

func main() {

	http.HandleFunc("/",
		func(w http.ResponseWriter, r *http.Request) {
			fmt.Fprintf(w, "Welcome to first GoLang Web Program running using DAPR! "+time.Now().Format("2006-01-02 15:04:05")) //YYYY-MM-DD hh:mm:ss
		})

	http.HandleFunc("/greeting",
		func(w http.ResponseWriter, r *http.Request) {
			var event incomingEvent
			decoder := json.NewDecoder(r.Body)
			decoder.Decode(&event)
			fmt.Println(event.Data)
			fmt.Fprintf(w, "Welcome, Greetings of the day from DAPR !! "+time.Now().Format("2006-01-02 15:04:05"))
		})

	http.HandleFunc("/greetme",
		func(w http.ResponseWriter, r *http.Request) {
			var event incomingEvent
			decoder := json.NewDecoder(r.Body)
			decoder.Decode(&event)
			fmt.Println(event.Data)

			keys, ok := r.URL.Query()["name"]
			if !ok || len(keys[0]) < 1 {
				log.Println("Url Param 'name' is missing")
				return
			}
			// Query()["key"] will return an array of items,
			// we only want the single item.
			name := keys[0]

			fmt.Fprintf(w, "Welcome "+string(name)+", Greetings of the day from DAPR !! "+time.Now().Format("2006-01-02 15:04:05"))
		})

	log.Fatal(http.ListenAndServe(":8088", nil))
}
