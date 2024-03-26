import React, { useState, useEffect } from 'react';
import { Container, Form, Button } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';

const Checkout = ({ cartItems }) => {
    const [address, setAddress] = useState('');
    const [paymentMethod, setPaymentMethod] = useState('Cash On Delivery');
    const [totalAmount, setTotalAmount] = useState(0);
    const navigate = useNavigate();

    useEffect(() => {
        // Calculate total amount from cart items
        const total = cartItems.reduce((acc, item) => acc + item.price, 0);
        setTotalAmount(total);
    }, [cartItems]);

    const handleSubmit = async () => {
        try {
            const loggedInUser = sessionStorage.getItem('username');

            // Prepare order object
            const orderData = {
                userName: loggedInUser, // Include the username in the order data
                orderAddress: address,
                totalAmount,
                orderStatus: 'Pending', // Assuming the order status starts as pending
            };

            // Make API call to place the order
            const response = await fetch('https://localhost:7075/api/Order', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(orderData)
            });

            if (response.ok) {
                console.log('Order placed successfully');
                // Navigate to checkout success page
                navigate('/checkout-success');
            } else {
                console.error('Failed to place order:', response.statusText);
            }
        } catch (error) {
            console.error('Error placing order:', error);
        }
    };

    return (
        <Container>
            <h2 className="my-4">Checkout</h2>
            <Form>
                <Form.Group controlId="formAddress">
                    <Form.Label>Address</Form.Label>
                    <Form.Control type="text" placeholder="Enter your address" value={address} onChange={(e) => setAddress(e.target.value)} />
                </Form.Group>
                <Form.Group controlId="formPaymentMethod">
                    <Form.Label>Payment Method</Form.Label>
                    <Form.Control as="select" value={paymentMethod} onChange={(e) => setPaymentMethod(e.target.value)}>
                        <option>Cash On Delivery</option>
                        {/* Add other payment methods if needed */}
                    </Form.Control>
                </Form.Group>
            </Form>
            <p>Total Amount: ${totalAmount}</p>
            <Button variant="primary" onClick={handleSubmit}>Order Now</Button>
        </Container>
    );
};

export default Checkout;

Cart.jsx:

import React, { useState, useEffect } from 'react';
import { Container, Table, Button } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';

const Cart = () => {
    const [cartItems, setCartItems] = useState([]);
    const navigate = useNavigate();

    useEffect(() => {
        fetchCartItems();
    }, []);

    const fetchCartItems = async () => {
        try {
            const loggedInUser = sessionStorage.getItem('username');

            // Fetch cart items for the logged in user
            const cartResponse = await fetch(`https://localhost:7075/api/Cart/CartByName/${loggedInUser}`);
            const cartData = await cartResponse.json();

            // Fetch product details for each item in the cart
            const cartItemsWithDetails = await Promise.all(
                cartData.map(async (item) => {
                    // Fetch product details using productId
                    const productResponse = await fetch(`https://localhost:7075/api/Product/ProductById/${item.productId}`);
                    const productData = await productResponse.json();

                    return {
                        ...item,
                        image: productData.imagePath, // Assuming the image path is provided by the API
                        name: productData.productName,
                        price: productData.price
                    };
                })
            );

            setCartItems(cartItemsWithDetails);
        } catch (error) {
            console.error('Error fetching cart items:', error);
        }
    };

    const removeFromCart = async (productId) => {
        try {
            const loggedInUser = sessionStorage.getItem('username');
            const response = await fetch(`https://localhost:7075/api/Cart/Remove?username=${loggedInUser}&productId=${productId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                },
            });
            if (response.ok) {
                console.log('Item removed from cart:', productId);
                fetchCartItems(); // Fetch updated cart items after removing
            } else {
                console.error('Failed to remove item from cart:', response.statusText);
            }
        } catch (error) {
            console.error('Error removing item from cart:', error);
        }
    };

    const checkout = () => {
        navigate('/Checkout'); // Navigate to checkout page
    };

    return (
        <Container>
            <h2 className="my-4">Cart</h2>
            {cartItems.length > 0 ? (
                <Table striped bordered hover>
                    <thead>
                        <tr>
                            <th>Image</th>
                            <th>Name</th>
                            <th>Price</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {cartItems.map((item, index) => (
                            <tr key={`${item.username}-${item.productId}-${index}`}>
                                <td><img src={`/assets/${item.image}`} alt={item.name} style={{ width: '50px', height: 'auto' }} /></td>
                                <td>{item.name}</td>
                                <td>${item.price}</td>
                                <td>
                                    <Button variant="danger" onClick={() => removeFromCart(item.productId)}>Remove</Button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </Table>
            ) : (
                <p>Your cart is empty.</p>
            )}
            <Button variant="primary" onClick={checkout}>Checkout</Button>
        </Container>
    );
};

export default Cart;

App.jsx:
import React, { useState } from "react";
import { BrowserRouter as Router, Route, Routes, Navigate } from "react-router-dom";
// Import other components...
import Cart from "./Cart";
import Checkout from "./Checkout"; // Import the Checkout component

const App = () => {
    const [cartItems, setCartItems] = useState([]);
    // Other state variables and functions...

    // useEffect or other logic to fetch and set cart items...

    return (
        <Router>
            {/* Other components... */}
            <Routes>
                {/* Other routes... */}
                <Route path="/Cart" element={<Cart cartItems={cartItems} />} />
                <Route path="/Checkout" element={<Checkout cartItems={cartItems} />} />
                {/* Other routes... */}
            </Routes>
            {/* Other elements... */}
        </Router>
    );
};

export default App;


          
