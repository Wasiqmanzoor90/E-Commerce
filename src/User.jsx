import axios from 'axios';
import React, { useEffect, useState } from 'react'

function User() {
  const [buyer, setbuyer] = useState([]);
  const fetchData = async () => {
    try {

      const response = await axios.get("http://localhost:5189/api/Admin/Dashboard", {
        headers: { "Content-Type": "application/json" },
        withCredentials: true,
      });
      const userData = response.data.buyers?.$values || []; // Access the array
      setbuyer(userData);
      console.log(userData);
    }
    catch (error) {
      console.log(error);
    }
  };
  useEffect(() => {
    fetchData();
  }, []);

  return (
    <div className="Container mx-3 my-2">
      <h3 className=' mb-3'>User List</h3>
      <table className='table table-bordered'>
        <thead>
          <tr>
            <th>Email</th>
            <th>Name</th>
            <th>Role</th>
            <th>Phone No</th>
            <th>State</th>
          </tr>
        </thead>
        <tbody>
          {buyer.map((buy) =>
           
              buy.address.$values.map((addr, index) => (
                <tr key={index}>
                  {index === 0 && (
                    <>
                      <td>{buy.email}</td>
                      <td>{buy.name}</td>
                      <td >{buy.role === 0 ? "Buyer" : "Seller"}</td>
                    </>
                  )}
                  <td>{addr.phone}</td>
                  <td>{addr.state}</td>
                </tr>
              ))
           
          )}
        </tbody>


      </table>

    </div>
  )
}

export default User