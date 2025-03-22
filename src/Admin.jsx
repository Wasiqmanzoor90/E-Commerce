import axios from 'axios';
import React, { useState, useEffect } from 'react'
function Admin() {
    const [cart, setCart] = useState([]);
    const [order, setOrder] = useState([]);

    const Fetchdata = async () => {
        const response = await axios.get("http://localhost:5189/api/Admin/Dashboard", {
            headers: { "Content-Type": "application/json" },
            withCredentials: true, // Ensure authentication cookies are sent
        });
        setCart(response.data.cart || []);
        setOrder(response.data.orders || []);
    }
    useEffect(() => {
        Fetchdata();
    }, []);
    return (
        <div className="container my-5" >
            <h2 className='mb-2'>Admin Panel</h2>
            <h3 className='mt-4'>Orders</h3>
            <table className='table table-striped table-hover'>
                <thead>
                    <tr>

                        <th>Order ID</th>
                        <th>Name</th>
                        <th>Order Date</th>
                        <th>Total Amount</th>
                        <th>Address</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>

                    {order.map((order) => (
                        <tr key={order.orderId}>
                            <td>{order.orderId}</td>
                            <td>{order.address?.firstName}</td>
                            <td>
                                {order.address?.dateCreated
                                    ? new Date(order.address.dateCreated).toLocaleDateString("en-GB", { day: "2-digit", month: "short", year: "numeric" })
                                    : "N/A"}
                            </td>
                            <td>₹ {order.orderPrice?.toFixed(2)}</td>
                            <td>
                                {order.address && `${order.address.street1}, ${order.address.pincode}`}
                            </td>
                            <td>{order.status}</td>
                        </tr>
                    ))}
                </tbody>
            </table>




            <h3 className='mt-4'>Cart items</h3>
            <table className='table table-striped table-hover'>
                <thead>
                    <tr>
                        <th>CartId</th>
                        <th>UserId</th>
                        <th>CartItemId</th>
                        <th>Quantity</th>
                    </tr>
                </thead>
                <tbody>
                    {cart?.$values?.length > 0 ? (
                        cart.$values.map((cartItem) =>
                            cartItem.cartItem?.$values?.map((cartProduct) => (
                                <tr key={cartProduct.cartItemId}>
                                    <td>{cartItem.cartId}</td>
                                    <td>{cartItem.userId}</td>
                                    <td>{cartProduct.cartItemId}</td>
                                    <td>{cartProduct.quantity}</td>
                                </tr>
                            ))
                        )
                    ) : (
                        <tr>
                            <td colSpan="4" className="text-center">No items in the cart</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    )
}

export default Admin