import React from 'react'
import { Link } from 'react-router-dom'

function Nav() {
  return (
    <div className=' container my-5 d-block justify-content-center' >

        <div className=' my-5'><h2>Admin Dashboard!</h2></div>
        <Link className='btn btn-lg btn-primary me-4' to={"/Seller"}>Seller List</Link>
        <Link className='btn btn-lg btn-primary me-4' to={"/User"}>Users List</Link>
        <Link className='btn btn-lg btn-primary me-4' to={"/Admin"}>Order List</Link>
       
        <Link className='btn btn-lg btn-primary me-4' to={"/Product"}>Product List</Link>
    </div>
  )
}

export default Nav