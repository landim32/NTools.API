import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { ThemeProvider } from "@/hooks/use-theme";
import { Navigation } from "@/components/Navigation";
import { Footer } from "@/components/Footer";
import Index from "./pages/Index";
import Login from "./pages/Login";
import Signup from "./pages/Signup";
import Register from "./pages/Register";
import ChangePassword from "./pages/ChangePassword";
import RecoverPassword from "./pages/RecoverPassword";
import NotFound from "./pages/NotFound";
import { ContextBuilder, UserProvider } from "./lib/nauth-core";

const queryClient = new QueryClient();

  const ContextContainer = ContextBuilder([
    UserProvider
  ]);

const App = () => (
  <ContextContainer>
  <QueryClientProvider client={queryClient}>
    <ThemeProvider defaultTheme="light" storageKey="nauth-ui-theme">
      <TooltipProvider>
        <Toaster />
        <Sonner />
        <BrowserRouter basename="/nauth">
          <div className="min-h-screen flex flex-col bg-background">
            <Navigation />
            <main className="flex-1">
              <Routes>
                <Route path="/" element={<Index />} />
                <Route path="/login" element={<Login />} />
                <Route path="/signup" element={<Signup />} />
                <Route path="/register" element={<Register />} />
                <Route path="/change-password" element={<ChangePassword />} />
                <Route path="/recover-password" element={<RecoverPassword />} />
                <Route path="*" element={<NotFound />} />
              </Routes>
            </main>
            <Footer />
          </div>
        </BrowserRouter>
      </TooltipProvider>
    </ThemeProvider>
  </QueryClientProvider>
  </ContextContainer>
);

export default App;
