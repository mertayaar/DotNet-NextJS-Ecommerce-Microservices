"use client"

import { useEffect } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { Button } from "@/components/ui/button"
import {
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { Loader2, MapPin, Star } from "lucide-react"
import { Address } from "@/services/addressService"
import Link from "next/link"

const phoneRegex = new RegExp(
    /^([+]?[\s0-9]+)?(\d{3}|[(]?[0-9]+[)])?([-]?[\s]?[0-9])+$/
);

const formSchema = z.object({
    name: z.string().min(2, "Name must be at least 2 characters"),
    surname: z.string().min(2, "Surname must be at least 2 characters"),
    email: z.string().email("Invalid email address"),
    phone: z.string().regex(phoneRegex, "Invalid phone number"),
    city: z.string().min(2, "City is required"),
    district: z.string().min(2, "District is required"),
    address: z.string().min(10, "Address must be at least 10 characters"),
})

export type CheckoutFormValues = z.infer<typeof formSchema>

interface CheckoutFormProps {
    onSubmit: (data: CheckoutFormValues) => void
    isLoading?: boolean
    savedAddresses?: Address[]
    onAddressSelect?: (address: Address | null) => void
}

export function CheckoutForm({ onSubmit, isLoading, savedAddresses = [], onAddressSelect }: CheckoutFormProps) {
    const form = useForm<CheckoutFormValues>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: "",
            surname: "",
            email: "",
            phone: "",
            city: "",
            district: "",
            address: "",
        },
    })

    
    useEffect(() => {
        const defaultAddress = savedAddresses.find(a => a.isDefault)
        if (defaultAddress) {
            fillFormWithAddress(defaultAddress)
        }
    }, [savedAddresses])

    const fillFormWithAddress = (address: Address) => {
        form.reset({
            name: address.name,
            surname: address.surname,
            email: address.email || "",
            phone: address.phone,
            city: address.city,
            district: address.district,
            address: address.addressLine,
        })
        onAddressSelect?.(address)
    }

    const handleAddressChange = (value: string) => {
        if (value === "new") {
            form.reset({
                name: "",
                surname: "",
                email: "",
                phone: "",
                city: "",
                district: "",
                address: "",
            })
            onAddressSelect?.(null)
        } else {
            const address = savedAddresses.find(a => a.id.toString() === value)
            if (address) {
                fillFormWithAddress(address)
            }
        }
    }

    const defaultAddress = savedAddresses.find(a => a.isDefault)

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
                {}
                {savedAddresses.length > 0 && (
                    <div className="space-y-2 pb-4 border-b">
                        <div className="flex items-center justify-between">
                            <FormLabel className="text-base font-medium">Saved Addresses</FormLabel>
                            <Link
                                href="/profile/addresses"
                                className="text-xs text-primary hover:underline"
                            >
                                Manage Addresses
                            </Link>
                        </div>
                        <Select
                            onValueChange={handleAddressChange}
                            defaultValue={defaultAddress?.id.toString()}
                        >
                            <SelectTrigger>
                                <SelectValue placeholder="Select a saved address or enter new" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="new" textValue="Enter new address">
                                    <span className="flex items-center gap-2">
                                        <MapPin className="h-4 w-4" />
                                        Enter new address
                                    </span>
                                </SelectItem>
                                {savedAddresses.map((addr) => (
                                    <SelectItem key={addr.id} value={addr.id.toString()} textValue={addr.title}>
                                        <span className="flex items-center gap-2">
                                            <MapPin className="h-4 w-4" />
                                            {addr.title}
                                            {addr.isDefault && <Star className="h-3 w-3 text-yellow-500 fill-yellow-500" />}
                                        </span>
                                    </SelectItem>
                                ))}
                            </SelectContent>
                        </Select>
                    </div>
                )}

                {}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <FormField
                        control={form.control}
                        name="name"
                        render={({ field }) => (
                            <FormItem className="relative pb-8">
                                <FormLabel>Name</FormLabel>
                                <FormControl>
                                    <Input placeholder="John" {...field} />
                                </FormControl>
                                <FormMessage className="absolute bottom-0 text-xs" />
                            </FormItem>
                        )}
                    />
                    <FormField
                        control={form.control}
                        name="surname"
                        render={({ field }) => (
                            <FormItem className="relative pb-8">
                                <FormLabel>Surname</FormLabel>
                                <FormControl>
                                    <Input placeholder="Doe" {...field} />
                                </FormControl>
                                <FormMessage className="absolute bottom-0 text-xs" />
                            </FormItem>
                        )}
                    />
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <FormField
                        control={form.control}
                        name="email"
                        render={({ field }) => (
                            <FormItem className="relative pb-8">
                                <FormLabel>Email</FormLabel>
                                <FormControl>
                                    <Input placeholder="john.doe@example.com" type="email" {...field} />
                                </FormControl>
                                <FormMessage className="absolute bottom-0 text-xs" />
                            </FormItem>
                        )}
                    />
                    <FormField
                        control={form.control}
                        name="phone"
                        render={({ field }) => (
                            <FormItem className="relative pb-8">
                                <FormLabel>Phone</FormLabel>
                                <FormControl>
                                    <Input placeholder="+1234567890" {...field} />
                                </FormControl>
                                <FormMessage className="absolute bottom-0 text-xs" />
                            </FormItem>
                        )}
                    />
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <FormField
                        control={form.control}
                        name="city"
                        render={({ field }) => (
                            <FormItem className="relative pb-8">
                                <FormLabel>City</FormLabel>
                                <FormControl>
                                    <Input placeholder="New York" {...field} />
                                </FormControl>
                                <FormMessage className="absolute bottom-0 text-xs" />
                            </FormItem>
                        )}
                    />
                    <FormField
                        control={form.control}
                        name="district"
                        render={({ field }) => (
                            <FormItem className="relative pb-8">
                                <FormLabel>District</FormLabel>
                                <FormControl>
                                    <Input placeholder="Manhattan" {...field} />
                                </FormControl>
                                <FormMessage className="absolute bottom-0 text-xs" />
                            </FormItem>
                        )}
                    />
                </div>

                <FormField
                    control={form.control}
                    name="address"
                    render={({ field }) => (
                        <FormItem className="relative pb-8">
                            <FormLabel>Address</FormLabel>
                            <FormControl>
                                <Textarea
                                    placeholder="123 Main St, Apt 4B"
                                    className="resize-none h-24"
                                    {...field}
                                />
                            </FormControl>
                            <FormMessage className="absolute bottom-0 text-xs" />
                        </FormItem>
                    )}
                />

                <Button type="submit" className="w-full" size="lg" disabled={isLoading}>
                    {isLoading ? (
                        <>
                            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                            Processing...
                        </>
                    ) : (
                        "Complete Order"
                    )}
                </Button>
            </form>
        </Form>
    )
}
