package main

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"os"
	"time"
)

type Item struct {
	Name  string `json:"name"`
	Price int    `json:"price"`
	Slots int    `json:"slots"`
}

type ItemDetails struct {
	Name         string `json:"name"`
	Price        int    `json:"price"`
	PricePerSlot int    `json:"pricePerSlot"`
	Tier         string `json:"tier"`
}

func health(w http.ResponseWriter, req *http.Request) {
	w.WriteHeader(http.StatusOK)
}

func itemDetails(w http.ResponseWriter, req *http.Request) {

	client := http.Client{
		Timeout: 2 * time.Second,
	}

	itemName := req.URL.Query().Get("itemName")
	url := "https://tarkov-market.com/api/v1/item"
	requestBody, err := json.Marshal(map[string]string{
		"q":    itemName,
		"lang": "en",
	})
	if err != nil {
		log.Println(err)
		w.WriteHeader(http.StatusInternalServerError)
		return
	}

	r, err := http.NewRequest("POST", url, bytes.NewBuffer(requestBody))
	if err != nil {
		log.Println(err)
		w.WriteHeader(http.StatusInternalServerError)
		return
	}
	apiKey := os.Getenv("EFT_MARKET_API_KEY")
	fmt.Println(apiKey)
	r.Header.Set("x-api-key", apiKey)
	r.Header.Set("Content-Type", "application/json")

	resp, err := client.Do(r)
	if err != nil {
		log.Println(err)
		w.WriteHeader(http.StatusInternalServerError)
		return
	}
	defer resp.Body.Close()

	body, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		w.WriteHeader(http.StatusInternalServerError)
		log.Println(err)
		return
	}
	var items []Item
	json.Unmarshal(body, &items)

	if len(items) == 0 {
		w.WriteHeader(http.StatusBadRequest)
		return
	}

	var itemDetails ItemDetails
	itemDetails.Name = items[0].Name
	itemDetails.Price = items[0].Price
	itemDetails.PricePerSlot = items[0].Price / items[0].Slots
	itemDetails.Tier = getItemTier(itemDetails.PricePerSlot)

	itemDetailsByte, err := json.Marshal(itemDetails)
	if err != nil {
		log.Println(err)
		w.WriteHeader(http.StatusInternalServerError)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	w.Write(itemDetailsByte)
}

func getItemTier(price int) string {
	if price > 0 && price < 10000 {
		return "F"
	} else if price >= 10000 && price < 20000 {
		return "D"
	} else if price >= 20000 && price < 30000 {
		return "C"
	} else if price >= 30000 && price < 40000 {
		return "B"
	} else if price >= 40000 && price < 50000 {
		return "A"
	} else {
		return "S"
	}
}

func main() {

	http.HandleFunc("/health", health)
	http.HandleFunc("/item-details", itemDetails)
	http.ListenAndServe(":8090", nil)
}
