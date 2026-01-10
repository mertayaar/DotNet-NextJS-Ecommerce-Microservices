"use client"

import { useEffect, useState } from "react"
import { useRouter } from "next/navigation"
import Link from "next/link"
import { useAuth } from "@/contexts/AuthContext"
import {
    Address,
    CreateAddressData,
    getAddresses,
    createAddress,
    updateAddress,
    deleteAddress,
    setDefaultAddress
} from "@/services/addressService"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { Checkbox } from "@/components/ui/checkbox"
import {
    Card,
    CardContent,
    CardFooter,
    CardHeader,
    CardTitle,
} from "@/components/ui/card"
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from "@/components/ui/dialog"
import { toast } from "sonner"
import { MapPin, Plus, Pencil, Trash2, Star, ArrowLeft, Loader2 } from "lucide-react"

interface AddressFormData {
    title: string;
    name: string;
    surname: string;
    email: string;
    phone: string;
    city: string;
    district: string;
    addressLine: string;
    isDefault: boolean;
}

const emptyForm: AddressFormData = {
    title: "",
    name: "",
    surname: "",
    email: "",
    phone: "",
    city: "",
    district: "",
    addressLine: "",
    isDefault: false
};

export default function AddressesPage() {
    const { isAuthenticated, isLoading: authLoading } = useAuth()
    const router = useRouter()
    const [addresses, setAddresses] = useState<Address[]>([])
    const [isLoading, setIsLoading] = useState(true)
    const [dialogOpen, setDialogOpen] = useState(false)
    const [editingAddress, setEditingAddress] = useState<Address | null>(null)
    const [formData, setFormData] = useState<AddressFormData>(emptyForm)
    const [isSubmitting, setIsSubmitting] = useState(false)

    useEffect(() => {
        if (!authLoading && !isAuthenticated) {
            router.push("/login?returnUrl=/profile/addresses")
        }
    }, [authLoading, isAuthenticated, router])

    useEffect(() => {
        if (isAuthenticated) {
            loadAddresses()
        }
    }, [isAuthenticated])

    const loadAddresses = async () => {
        setIsLoading(true)
        const data = await getAddresses()
        setAddresses(data)
        setIsLoading(false)
    }

    const handleOpenDialog = (address?: Address) => {
        if (address) {
            setEditingAddress(address)
            setFormData({
                title: address.title,
                name: address.name,
                surname: address.surname,
                email: address.email,
                phone: address.phone,
                city: address.city,
                district: address.district,
                addressLine: address.addressLine,
                isDefault: address.isDefault
            })
        } else {
            setEditingAddress(null)
            setFormData(emptyForm)
        }
        setDialogOpen(true)
    }

    const handleCloseDialog = () => {
        setDialogOpen(false)
        setEditingAddress(null)
        setFormData(emptyForm)
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        setIsSubmitting(true)

        const data: CreateAddressData = {
            title: formData.title,
            name: formData.name,
            surname: formData.surname,
            email: formData.email,
            phone: formData.phone,
            city: formData.city,
            district: formData.district,
            addressLine: formData.addressLine,
            isDefault: formData.isDefault
        }

        let result;
        if (editingAddress) {
            result = await updateAddress(editingAddress.id, data)
            if (result.success) {
                toast.success("Address updated successfully")
            }
        } else {
            result = await createAddress(data)
            if (result.success) {
                toast.success("Address created successfully")
            }
        }

        setIsSubmitting(false)

        if (result.success) {
            handleCloseDialog()
            loadAddresses()
        } else {
            toast.error(result.message || "Operation failed")
        }
    }

    const handleDelete = async (id: number, title: string) => {
        toast(`Delete "${title || 'this address'}"?`, {
            description: "This action cannot be undone.",
            action: {
                label: "Delete",
                onClick: async () => {
                    const result = await deleteAddress(id)
                    if (result.success) {
                        toast.success("Address deleted")
                        loadAddresses()
                    } else {
                        toast.error(result.message || "Failed to delete address")
                    }
                }
            },
            cancel: {
                label: "Cancel",
                onClick: () => { }
            }
        })
    }

    const handleSetDefault = async (id: number) => {
        const result = await setDefaultAddress(id)
        if (result.success) {
            toast.success("Default address updated")
            loadAddresses()
        } else {
            toast.error(result.message || "Failed to set default")
        }
    }

    if (authLoading || isLoading) {
        return (
            <div className="flex h-screen items-center justify-center">
                <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
            </div>
        )
    }

    return (
        <div className="container py-10 max-w-4xl">
            <div className="flex items-center gap-4 mb-8">
                <Button variant="ghost" asChild className="pl-0">
                    <Link href="/profile" className="flex items-center gap-2 text-muted-foreground hover:text-foreground">
                        <ArrowLeft className="h-4 w-4" />
                        Back to Profile
                    </Link>
                </Button>
            </div>

            <div className="flex justify-between items-center mb-8">
                <div>
                    <h1 className="text-3xl font-bold tracking-tight">Address Book</h1>
                    <p className="text-muted-foreground mt-1">Manage your saved addresses for faster checkout</p>
                </div>
                <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
                    <DialogTrigger asChild>
                        <Button onClick={() => handleOpenDialog()}>
                            <Plus className="h-4 w-4 mr-2" />
                            Add Address
                        </Button>
                    </DialogTrigger>
                    <DialogContent className="sm:max-w-[500px]">
                        <form onSubmit={handleSubmit}>
                            <DialogHeader>
                                <DialogTitle>
                                    {editingAddress ? "Edit Address" : "Add New Address"}
                                </DialogTitle>
                                <DialogDescription>
                                    {editingAddress
                                        ? "Update your address details below."
                                        : "Add a new address to your address book."}
                                </DialogDescription>
                            </DialogHeader>
                            <div className="grid gap-4 py-4">
                                <div className="space-y-2">
                                    <Label htmlFor="title">Address Title</Label>
                                    <Input
                                        id="title"
                                        placeholder="e.g., Home, Work, Mom's House"
                                        value={formData.title}
                                        onChange={e => setFormData({ ...formData, title: e.target.value })}
                                        required
                                    />
                                </div>
                                <div className="grid grid-cols-2 gap-4">
                                    <div className="space-y-2">
                                        <Label htmlFor="name">Name</Label>
                                        <Input
                                            id="name"
                                            placeholder="John"
                                            value={formData.name}
                                            onChange={e => setFormData({ ...formData, name: e.target.value })}
                                            required
                                        />
                                    </div>
                                    <div className="space-y-2">
                                        <Label htmlFor="surname">Surname</Label>
                                        <Input
                                            id="surname"
                                            placeholder="Doe"
                                            value={formData.surname}
                                            onChange={e => setFormData({ ...formData, surname: e.target.value })}
                                            required
                                        />
                                    </div>
                                </div>
                                <div className="grid grid-cols-2 gap-4">
                                    <div className="space-y-2">
                                        <Label htmlFor="email">Email</Label>
                                        <Input
                                            id="email"
                                            type="email"
                                            placeholder="john@example.com"
                                            value={formData.email}
                                            onChange={e => setFormData({ ...formData, email: e.target.value })}
                                        />
                                    </div>
                                    <div className="space-y-2">
                                        <Label htmlFor="phone">Phone</Label>
                                        <Input
                                            id="phone"
                                            placeholder="+1234567890"
                                            value={formData.phone}
                                            onChange={e => setFormData({ ...formData, phone: e.target.value })}
                                            required
                                        />
                                    </div>
                                </div>
                                <div className="grid grid-cols-2 gap-4">
                                    <div className="space-y-2">
                                        <Label htmlFor="city">City</Label>
                                        <Input
                                            id="city"
                                            placeholder="New York"
                                            value={formData.city}
                                            onChange={e => setFormData({ ...formData, city: e.target.value })}
                                            required
                                        />
                                    </div>
                                    <div className="space-y-2">
                                        <Label htmlFor="district">District</Label>
                                        <Input
                                            id="district"
                                            placeholder="Manhattan"
                                            value={formData.district}
                                            onChange={e => setFormData({ ...formData, district: e.target.value })}
                                            required
                                        />
                                    </div>
                                </div>
                                <div className="space-y-2">
                                    <Label htmlFor="addressLine">Address</Label>
                                    <Textarea
                                        id="addressLine"
                                        placeholder="123 Main St, Apt 4B"
                                        value={formData.addressLine}
                                        onChange={e => setFormData({ ...formData, addressLine: e.target.value })}
                                        required
                                        className="resize-none h-20"
                                    />
                                </div>
                                <div className="flex items-center space-x-2">
                                    <Checkbox
                                        id="isDefault"
                                        checked={formData.isDefault}
                                        onCheckedChange={(checked: boolean) => setFormData({ ...formData, isDefault: checked })}
                                    />
                                    <Label htmlFor="isDefault" className="text-sm font-normal cursor-pointer">
                                        Set as default address
                                    </Label>
                                </div>
                            </div>
                            <DialogFooter>
                                <Button type="button" variant="outline" onClick={handleCloseDialog}>
                                    Cancel
                                </Button>
                                <Button type="submit" disabled={isSubmitting}>
                                    {isSubmitting ? (
                                        <>
                                            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                                            Saving...
                                        </>
                                    ) : (
                                        editingAddress ? "Update" : "Add Address"
                                    )}
                                </Button>
                            </DialogFooter>
                        </form>
                    </DialogContent>
                </Dialog>
            </div>

            {addresses.length === 0 ? (
                <Card className="text-center py-12">
                    <CardContent>
                        <MapPin className="h-12 w-12 mx-auto text-muted-foreground mb-4" />
                        <h3 className="text-lg font-medium mb-2">No addresses saved</h3>
                        <p className="text-muted-foreground mb-4">
                            Add your first address for faster checkout
                        </p>
                        <Button onClick={() => handleOpenDialog()}>
                            <Plus className="h-4 w-4 mr-2" />
                            Add Address
                        </Button>
                    </CardContent>
                </Card>
            ) : (
                <div className="grid gap-4 md:grid-cols-2">
                    {addresses.map((address) => (
                        <Card key={address.id} className={address.isDefault ? "border-primary" : ""}>
                            <CardHeader className="pb-3">
                                <div className="flex items-center justify-between">
                                    <CardTitle className="text-lg flex items-center gap-2">
                                        <MapPin className="h-4 w-4" />
                                        {address.title}
                                    </CardTitle>
                                    {address.isDefault && (
                                        <span className="flex items-center gap-1 text-xs bg-primary text-primary-foreground px-2 py-1 rounded-full">
                                            <Star className="h-3 w-3" />
                                            Default
                                        </span>
                                    )}
                                </div>
                            </CardHeader>
                            <CardContent className="text-sm text-muted-foreground space-y-1">
                                <p className="font-medium text-foreground">
                                    {address.name} {address.surname}
                                </p>
                                <p>{address.addressLine}</p>
                                <p>{address.district}, {address.city}</p>
                                <p>{address.phone}</p>
                                {address.email && <p>{address.email}</p>}
                            </CardContent>
                            <CardFooter className="flex gap-2 pt-0">
                                <Button
                                    variant="outline"
                                    size="sm"
                                    onClick={() => handleOpenDialog(address)}
                                >
                                    <Pencil className="h-3 w-3 mr-1" />
                                    Edit
                                </Button>
                                {!address.isDefault && (
                                    <Button
                                        variant="outline"
                                        size="sm"
                                        onClick={() => handleSetDefault(address.id)}
                                    >
                                        <Star className="h-3 w-3 mr-1" />
                                        Set Default
                                    </Button>
                                )}
                                <Button
                                    variant="outline"
                                    size="sm"
                                    className="text-destructive hover:text-destructive"
                                    onClick={() => handleDelete(address.id, address.title)}
                                >
                                    <Trash2 className="h-3 w-3 mr-1" />
                                    Delete
                                </Button>
                            </CardFooter>
                        </Card>
                    ))}
                </div>
            )}
        </div>
    )
}
