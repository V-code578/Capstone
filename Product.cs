import React, { useState, useEffect } from 'react';
import { Card, Button, Container, Row, Col } from 'react-bootstrap';

const MensShirts = () => {
    const [products, setProducts] = useState([]);

    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
        try {
            const response = await fetch('http://localhost:5183/api/Product/AllProducts');
            const data = await response.json();
            setProducts(data);
        } catch (error) {
            console.error('Error fetching data:', error);
        }
    };

    const addToCart = (productId) => {
        // Implement your logic to add the product to the cart
        console.log('Product added to cart:', productId);
    };

    const addToWishlist = (productId) => {
        // Implement your logic to add the product to the wishlist
        console.log('Product added to wishlist:', productId);
    };

    return (
        <Container>
            <h1 className="my-4">Men's Shirts</h1>
            <Row>
                {products.map(product => (
                    <Col key={product.productId} md={4} sm={6} xs={12} className="mb-4">
                        <Card>
                            <Card.Img variant="top" src={product.imagePath} alt={product.productName} />
                            <Card.Body>
                                <Card.Title>{product.productName}</Card.Title>
                                <Card.Text>{product.description}</Card.Text>
                                <Card.Text>Price: ${product.price}</Card.Text>
                                <Button variant="primary" onClick={() => addToCart(product.productId)}>Add to Cart</Button>{' '}
                                <Button variant="secondary" onClick={() => addToWishlist(product.productId)}>Add to Wishlist</Button>
                            </Card.Body>
                        </Card>
                    </Col>
                ))}
            </Row>
        </Container>
    );
};

export default MensShirts;

npm install react-bootstrap bootstrap
