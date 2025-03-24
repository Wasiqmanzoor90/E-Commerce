import axios from 'axios';
import React, { useState, useEffect } from 'react';

function Seller() {
    const [seller, setSeller] = useState([]);

    const fetchData = async () => {
        try {
            const response = await axios.get("http://localhost:5189/api/Admin/Dashboard", {
                headers: { "Content-Type": "application/json" },
                withCredentials: true, // Ensure authentication cookies are sent
            });

            const userData = response.data.user?.$values || []; // Access the array
            setSeller(userData);
            console.log(userData);
        } catch (error) {
            console.error("Error fetching seller data:", error);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    return (
        <div className="Container mx-3 my-2">
            <h3>Seller List</h3>
            <table className="table table-bordered">
                <thead>
                    <tr>
                        <th>Email</th>
                        <th>Name</th>
                        <th>Role</th>
                        <th>State</th>
                        <th>Pincode</th>
                    </tr>
                </thead>
                <tbody>
                    {seller.map((sel) =>
                        sel.address?.$values?.map((addr, index) => (
                            <tr key={`${sel.id}-${index}`}>
                                <td>{sel.email || "N/A"}</td>
                                <td>{sel.name || "N/A"}</td>
                                <td>{sel.role || "N/A"}</td>
                                <td>{addr.state || "N/A"}</td>
                                <td>{addr.pincode || "N/A"}</td>
                            </tr>
                        ))
                    )}
                </tbody>


            </table>
        </div>
    );
}

export default Seller;
