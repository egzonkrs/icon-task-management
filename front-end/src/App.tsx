import { AuthProvider } from "@/context/AuthContext";
import { TicketProvider } from "@/context/TicketContext";
import { Layout } from "@/components/layout";
import { LoginModal, RegisterModal } from "@/components/auth";
import { Dashboard } from "@/pages/Dashboard";

export default function App() {
  return (
    <AuthProvider>
      <TicketProvider>
        <Layout>
          <Dashboard />
        </Layout>
        <LoginModal />
        <RegisterModal />
      </TicketProvider>
    </AuthProvider>
  );
}
