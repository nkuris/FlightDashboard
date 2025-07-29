import { useEffect, useState } from 'react';
import './App.css';
import Flights from './components/Flights';

function App() {
    const [flights, setFlights] = useState();

    useEffect(() => {
    }, []);

   
    return (
        <>
            <Flights />
        </>
    );
    
    
}

export default App;