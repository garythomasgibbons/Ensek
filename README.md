# Ensek

## Issues 
 - Getting to token is unreliable sometimes a 500 is returned
 - Only POST /reset requires the proper auth token
 - Using the PUT to buy energy units greater than the amount available you can update the "quantity_of_units": to a negative value.
 - PUT to buy energy is not persisting GMT (although this seems to be fixed today 07/09/2022) 
 - PUT to buy energy is not persisting data correctly id is ID and Fuel is Elec (not sure if this is random event requires more investigation) see below for example in 
 electric, however happens for all other fuel types.
 
 **'{
    "Id": "f8c8152f-fde2-4ccf-b6d8-f35e183f376a",
    "fuel": "Elec",
    "quantity": 1,
    "time": "Wed, 07 Sep 2022 10:29:37 GMT"
  }'**
  
  **'{
    "fuel": "electric",
    "id": "080d9823-e874-4b5b-99ff-2021f2a59b25",
    "quantity": 23,
    "time": "Mon, 7 Feb 2022 00:01:24 GMT"
  }'**
  
  - PUT to buy energy: the message unit price is incorrect when buying 1 unit of oil (0.6 returned I expected 0.5 (from energy controller))
    
  **'{
  "message": "You have purchased 1 Litres at a cost of 0.6 there are 19 units remaining. Your orderid is fdbb4b4f-fb8d-4bb3-82cd-12fc9f8be31a."
  }'**
  
  **'"oil": {
    "energy_id": 4,
    "price_per_unit": 0.5,
    "quantity_of_units": 19,
    "unit_type": "Litres"
  }'**
  
  ## Extra Tests
  - Sql injection to the PUT controller
  - Performance tests, measuring against requirements.
  - Check that messages from each controller are relaying the correct information 
  - I would also like to have more requirements / acceptance criteria to ensure that the tests are correct
  - More tests on the login controller
  - More destructive testing (manual)
  
  ## Improvements
  - store the token and reuse (depending on token requirements)
