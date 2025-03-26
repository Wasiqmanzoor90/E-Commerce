import React from "react"; 
import Admin from './Admin'
import { BrowserRouter, Route, Routes } from "react-router-dom";
import Nav from "./Nav";
import User from "./User";
import Seller from "./Seller";
import Product from "./Product";





function App() {
  return (
<BrowserRouter>

<Routes>
  <Route path="/" element={<Nav/>} />
<Route path="/Admin" element ={<Admin/>}/>
<Route path="/User" element ={<User/>}/>
<Route path="/Seller" element ={<Seller/>}/>
<Route path="/Product" element={<Product/>}/>

</Routes>

</BrowserRouter>

  )
}

export default App