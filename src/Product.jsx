import axios from 'axios';
import React, { useEffect, useState } from 'react';

function Product() {
  const [productp, setProduct] = useState([]);

  const fetchdata = async () => {
    const response = await axios.get("http://localhost:5189/api/Admin/Dashboard", {
      headers: { "Content-Type": "application/json" },
      withCredentials: true,
    });

    // Extract products correctly
    const userdata = response.data.prod?.$values || [];
    setProduct(userdata);
    console.log(userdata);
  };

  useEffect(() => {
    fetchdata();
  }, []);

  return (
    <div className="container my-3">
      <table className="table table-bordered">
        <thead>
          <tr>
            <th>Seller Name</th>
            <th>Product Name</th>
            <th>Quantity</th>
            <th>Category</th>
            <th>Price</th>
          </tr>
        </thead>
        <tbody>
          {productp.map((proo) =>
          
              proo.product?.$values?.map((pro, index) => (
                <tr key={index}>
                  {/* If the product is multiple and user is one */}
                  {index === 0 ? <td rowSpan={proo.product.$values.length}>{proo.name}</td> : null}
                  <td>{pro.productName || "N/A"}</td>
                  <td>{pro.productQuantity || "N/A"}</td>
                  <td>{pro.category || "N/A"}</td>
                  <td>{pro.productPrice || "N/A"}</td>
                </tr>
              ))
       
          )}
        </tbody>
      </table>
    </div>
  );
}

export default Product;
