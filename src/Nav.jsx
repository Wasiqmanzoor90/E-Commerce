
import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Chart as ChartJS, ArcElement, Tooltip, Legend, BarElement, CategoryScale, LinearScale } from 'chart.js';
import { Doughnut, Bar } from 'react-chartjs-2';

ChartJS.register(ArcElement, Tooltip, Legend, BarElement, CategoryScale, LinearScale);

const AdminDashboard = () => {
  const [stats, setStats] = useState({
    sellers: 0,
    users: 0,
    orders: 0,
    products: 0,
    pendingOrders: 0,
    completedOrders: 0
  });

  useEffect(() => {
    // Simulated data fetch - replace with your actual API call
    const fetchData = async () => {
      try {
        // This is mock data - replace with your actual API response
        const mockData = {
          User: Array(15), // 15 sellers
          Buyers: Array(42), // 42 buyers
          orders: Array(78), // 78 total orders
          pendingOrders: Array(23), // 23 pending orders
          completedOrders: Array(45), // 45 completed orders
          Prod: [{ Product: Array(156) }] // 156 products
        };

        setStats({
          sellers: mockData.User?.length || 0,
          users: (mockData.Buyers?.length || 0) + (mockData.User?.length || 0),
          orders: mockData.orders?.length || 0,
          products: mockData.Prod?.flatMap(p => p.Product).length || 0,
          pendingOrders: mockData.pendingOrders?.length || 0,
          completedOrders: mockData.completedOrders?.length || 0
        });
      } catch (error) {
        console.error("Error fetching dashboard data:", error);
      }
    };
    fetchData();
  }, []);

  const orderStatusData = {
    labels: ['Pending', 'Processing', 'Completed'],
    datasets: [{
      data: [stats.pendingOrders, stats.orders - stats.pendingOrders - stats.completedOrders, stats.completedOrders],
      backgroundColor: ['#FF6384', '#36A2EB', '#4BC0C0'],
      hoverBackgroundColor: ['#FF6384', '#36A2EB', '#4BC0C0']
    }]
  };

  const salesData = {
    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
    datasets: [{
      label: 'Sales 2023',
      data: [65, 59, 80, 81, 56, 55],
      backgroundColor: 'rgba(54, 162, 235, 0.5)',
      borderColor: 'rgba(54, 162, 235, 1)',
      borderWidth: 1
    }]
  };
  return (
    <div className="d-flex">
      {/* Sidebar Navigation */}
      <div className="bg-white" style={{ width: '250px', minHeight: '100vh' }}>
        <div className="p-3">
          <h4 className="text-center mb-4">Admin Panel</h4>
          <ul className="nav flex-column">
            <li className="nav-item">
              <Link className="nav-link text-black active" to="/admin">Dashboard</Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link text-black" to="/Seller">Sellers</Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link text-black" to="/User">Users</Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link text-black" to="/Admin">Orders</Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link text-black" to="/Product">Products</Link>
            </li>
          
          </ul>
        </div>
      </div>
  
      {/* Main Content */}
      <div className="flex-grow-1 p-4">
        <h2 className="mb-4">Dashboard Overview</h2>
        
        {/* Stats Cards */}
        <div className="row mb-4">
          <div className="col-md-3 mb-3">
            <div className="card bg-primary text-white">
              <div className="card-body">
                <h5 className="card-title">Sellers</h5>
                <h2 className="card-text">{stats.sellers}</h2>
              </div>
            </div>
          </div>
          <div className="col-md-3 mb-3">
            <div className="card bg-success text-white">
              <div className="card-body">
                <h5 className="card-title">Users</h5>
                <h2 className="card-text">{stats.users}</h2>
              </div>
            </div>
          </div>
          <div className="col-md-3 mb-3">
            <div className="card bg-info text-white">
              <div className="card-body">
                <h5 className="card-title">Orders</h5>
                <h2 className="card-text">{stats.orders}</h2>
              </div>
            </div>
          </div>
          <div className="col-md-3 mb-3">
            <div className="card bg-warning text-dark">
              <div className="card-body">
                <h5 className="card-title">Products</h5>
                <h2 className="card-text">{stats.products}</h2>
              </div>
            </div>
          </div>
        </div>
  
        {/* Charts Row */}
        <div className="row">
          <div className="col-md-6 mb-4">
            <div className="card">
              <div className="card-body">
                <h5 className="card-title">Order Status</h5>
                <div style={{ height: '300px' }}>
                  <Doughnut 
                    data={orderStatusData} 
                    options={{ 
                      maintainAspectRatio: false,
                      plugins: {
                        legend: {
                          position: 'bottom'
                        }
                      }
                    }} 
                  />
                </div>
              </div>
            </div>
          </div>
          <div className="col-md-6 mb-4">
            <div className="card">
              <div className="card-body">
                <h5 className="card-title">Sales Overview</h5>
                <div style={{ height: '300px' }}>
                  <Bar 
                    data={salesData} 
                    options={{ 
                      maintainAspectRatio: false,
                      scales: {
                        y: {
                          beginAtZero: true
                        }
                      }
                    }} 
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
export default AdminDashboard;