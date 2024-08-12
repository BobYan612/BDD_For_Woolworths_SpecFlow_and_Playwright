Feature: Trolley Item List
As a user, I may add products into my carts, and modify the items in trolley list
then it may show the items as category list and summarize the total price

@regression
Scenario: Choose a product from browse to trolley
	Given the user has login in 
	And Trolley item list is empty
	When add "fresh fruit bananas yellow" to trolley under "Browse/Fruit & Veg/Fruit/Shop all Fruit" in "Browse Page"
	Then The "fresh fruit bananas yellow" in the trolley can be seen
	And The total price and subtotal price have been shown correctly

@regression
Scenario: Choose a product from speical & offers to trolley
	Given the user has login in 
	And Trolley item list is empty
	When add "Hellers Sausages Cheese & Bacon Precooked" to trolley under "" in "Specials & offers Page"
	Then The "Hellers Sausages Cheese & Bacon Precooked" in the trolley can be seen
	And The total price and subtotal price have been shown correctly

@regression
Scenario: Choose a product from low price to trolley
	Given the user has login in 
	And Trolley item list is empty
	When add "woolworths free farmed bone in pork shoulder roast NZ" to trolley under "" in "Low Price Page"
	Then The "woolworths free farmed bone in pork shoulder roast NZ" in the trolley can be seen
	And The total price and subtotal price have been shown correctly

@regression
Scenario: Update the quantity in trolley
	Given the user has login in 
	And Trolley item list is empty
	And add "Hellers Sausages Cheese & Bacon Precooked" to trolley under "" in "Specials & offers Page"
	And add "fresh fruit bananas yellow" to trolley under "Browse/Fruit & Veg/Fruit/Shop all Fruit" in "Browse Page"
	When click "plus" for "fresh fruit bananas yellow" in Trolley Page
	When set "5" to quantity for "Hellers Sausages Cheese & Bacon Precooked" in Trolley Page
	When click "minus" for "fresh fruit bananas yellow" in Trolley Page
	Then The total price and subtotal price have been shown correctly

@regression
Scenario: Remove one item in the trolley
	Given the user has login in 
	And Trolley item list is empty
	And add "Hellers Sausages Cheese & Bacon Precooked" to trolley under "" in "Specials & offers Page"
	And add "fresh fruit bananas yellow" to trolley under "Browse/Fruit & Veg/Fruit/Shop all Fruit" in "Browse Page"
	When click "remove" for "fresh fruit bananas yellow" in Trolley Page
	Then The total price and subtotal price have been shown correctly